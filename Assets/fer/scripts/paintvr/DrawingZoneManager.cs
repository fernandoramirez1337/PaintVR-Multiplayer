// DrawingZoneManager.cs
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class DrawingZoneManager : NetworkBehaviour
{
    public static DrawingZoneManager Instance { get; private set; }

    [Header("Configuración de Zona")]
    [SerializeField] private GameObject drawingZonePrefab;
    [SerializeField] private int maxZones = 4;

    [Header("Configuración de Réplica")]
    public List<BoxCollider> replicaZones;

    private NetworkList<NetworkObjectReference> spawnedZones;
    private static List<BoxCollider> zoneColliders = new List<BoxCollider>();
    private bool areMeshesEnabled = true;

    // --- NUEVO: Historial de trazos centralizado en el servidor ---
    private readonly Dictionary<int, Stack<NetworkObject>> serverStrokeHistory = new Dictionary<int, Stack<NetworkObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        spawnedZones = new NetworkList<NetworkObjectReference>();
    }

    public override void OnNetworkSpawn()
    {
        spawnedZones.OnListChanged += OnSpawnedZonesChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (spawnedZones != null) spawnedZones.OnListChanged -= OnSpawnedZonesChanged;
    }

    private void OnSpawnedZonesChanged(NetworkListEvent<NetworkObjectReference> changeEvent)
    {
        zoneColliders.Clear();
        foreach (var zoneRef in spawnedZones)
        {
            if (zoneRef.TryGet(out NetworkObject zoneNetworkObject) && zoneNetworkObject.GetComponent<BoxCollider>() is BoxCollider collider)
            {
                zoneColliders.Add(collider);
            }
        }
    }

    // --- NUEVO: Método llamado por el Pincel para registrar un trazo en el servidor ---
    public void RegisterStrokeOnServer(int zoneIndex, NetworkObject strokeContainer)
    {
        if (!IsServer) return; // Esta lógica solo se ejecuta en el servidor

        if (!serverStrokeHistory.ContainsKey(zoneIndex))
        {
            serverStrokeHistory[zoneIndex] = new Stack<NetworkObject>();
        }
        serverStrokeHistory[zoneIndex].Push(strokeContainer);
        Debug.Log($"[Server] Trazo registrado en la zona {zoneIndex}. Total de trazos en esta zona: {serverStrokeHistory[zoneIndex].Count}");
    }

    // --- NUEVO: RPC llamado por el BarController de CUALQUIER jugador ---
    [ServerRpc(RequireOwnership = false)]
    public void RequestUndoServerRpc(int zoneIndex)
    {
        if (serverStrokeHistory.ContainsKey(zoneIndex) && serverStrokeHistory[zoneIndex].Count > 0)
        {
            NetworkObject strokeToUndo = serverStrokeHistory[zoneIndex].Pop();
            if (strokeToUndo != null && strokeToUndo.IsSpawned)
            {
                Debug.Log($"[Server] Deshaciendo trazo {strokeToUndo.NetworkObjectId} en la zona {zoneIndex}.");
                strokeToUndo.Despawn(true); // Despawn lo destruye para todos los clientes
            }
        }
        else
        {
            Debug.Log($"[Server] No hay trazos para deshacer en la zona {zoneIndex}.");
        }
    }
    
    public void RequestSpawnZone(Vector3 position, Quaternion rotation)
    {
        SpawnZoneServerRpc(position, rotation);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnZoneServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams rpcParams = default)
    {
        if (spawnedZones.Count >= maxZones) return;
        if (drawingZonePrefab == null) return;
        
        GameObject zoneInstance = Instantiate(drawingZonePrefab, position, rotation);
        NetworkObject netObj = zoneInstance.GetComponent<NetworkObject>();
        netObj.Spawn(true);
        spawnedZones.Add(new NetworkObjectReference(netObj));
    }

    #region Métodos de Búsqueda (sin cambios)
    public void ToggleZoneMeshRenderers()
    {
        areMeshesEnabled = !areMeshesEnabled;
        foreach (var zoneRef in spawnedZones)
        {
            if (zoneRef.TryGet(out NetworkObject zoneNetworkObject))
            {
                var meshRenderer = zoneNetworkObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null) meshRenderer.enabled = areMeshesEnabled;
            }
        }
    }
    
    public static bool FindZoneAtPosition(Vector3 position, out NetworkObject zoneObject)
    {
        zoneObject = null;
        foreach (var collider in zoneColliders)
        {
            if (collider != null && collider.bounds.Contains(position))
            {
                zoneObject = collider.GetComponent<NetworkObject>();
                return true;
            }
        }
        return false;
    }

    public static bool TryGetReplicaForMaster(NetworkObject masterZoneObject, out BoxCollider replicaCollider, out int masterIndex)
    {
        replicaCollider = null;
        masterIndex = GetZoneIndex(masterZoneObject);

        if (masterIndex != -1 && Instance != null && masterIndex < Instance.replicaZones.Count)
        {
            replicaCollider = Instance.replicaZones[masterIndex];
            return replicaCollider != null;
        }

        return false;
    }
    
    public static bool TryGetZoneObjectByIndex(int index, out NetworkObject zoneObject)
    {
        zoneObject = null;
        if (Instance == null || index < 0 || index >= Instance.spawnedZones.Count) return false;
        return Instance.spawnedZones[index].TryGet(out zoneObject);
    }

    public static int GetZoneIndex(NetworkObject zoneObject)
    {
        if (Instance == null || zoneObject == null) return -1;
        for (int i = 0; i < Instance.spawnedZones.Count; i++)
        {
            if (Instance.spawnedZones[i].TryGet(out NetworkObject netObj) && netObj == zoneObject)
            {
                return i;
            }
        }
        return -1;
    }
    #endregion
}