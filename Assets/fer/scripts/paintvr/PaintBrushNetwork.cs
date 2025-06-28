// PaintBrushNetwork.cs
using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class PaintBrushNetwork : NetworkBehaviour
{
    [Header("Configuración de Dibujo")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private Material replicaLineMaterial;
    [SerializeField] private Transform drawingOriginPoint;
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private float spawnInterval = 0.01f;

    [Header("Configuración de Deshacer")]
    [SerializeField] private GameObject strokeContainerPrefab;

    [Header("Organización")]
    [SerializeField] private Transform strokesParent;
    
    [Header("Configuración de Sonido")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip drawingSound;
    [SerializeField] private float soundVolume = 0.5f;
    [SerializeField] private bool loopDrawingSound = true;
    
    private bool isGrabbed = false;
    private bool isPerformingGesture = false;
    private bool wasDrawingLastFrame = false;
    private bool isSoundPlaying = false;
    private Vector3 lastPosition;
    private float spawnTimer = 0;
    private NetworkObject currentStrokeContainer;
    private int lastDrawingZoneIndex = -1;
    
    // --- ELIMINADO: El historial de trazos ya no se guarda en el cliente ---
    // private readonly Dictionary<int, Stack<NetworkObject>> strokesByZone = new Dictionary<int, Stack<NetworkObject>>();

    private Vector3 OriginPosition => drawingOriginPoint != null ? drawingOriginPoint.position : transform.position;

    void Start()
    {
        // Configurar AudioSource si no se asignó uno
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // Configurar propiedades del AudioSource
        if (audioSource != null)
        {
            audioSource.volume = soundVolume;
            audioSource.loop = loopDrawingSound;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        bool canDrawNow = isGrabbed && isPerformingGesture;
        bool isInDrawingZone = DrawingZoneManager.FindZoneAtPosition(OriginPosition, out NetworkObject currentZoneObject);

        if (canDrawNow && isInDrawingZone)
        {
            if (!wasDrawingLastFrame)
            {
                lastPosition = OriginPosition;
                RequestNewStrokeContainerServerRpc();
                
                // Iniciar sonido de dibujo
                StartDrawingSound();
            }

            spawnTimer += Time.deltaTime;

            if (currentStrokeContainer != null && spawnTimer >= spawnInterval && Vector3.Distance(lastPosition, OriginPosition) > 0.001f)
            {
                lastDrawingZoneIndex = DrawingZoneManager.GetZoneIndex(currentZoneObject);
                
                Color colorToDraw = GetCurrentMaterialColor();
                var startPositions = new List<Vector3>() { lastPosition };
                var endPositions = new List<Vector3>() { OriginPosition };
                
                if (lastDrawingZoneIndex != -1 && DrawingZoneManager.TryGetReplicaForMaster(currentZoneObject, out BoxCollider replicaCollider, out _))
                {
                    Transform masterTransform = currentZoneObject.transform;
                    Transform replicaTransform = replicaCollider.transform;
                    startPositions.Add(replicaTransform.TransformPoint(masterTransform.InverseTransformPoint(lastPosition)));
                    endPositions.Add(replicaTransform.TransformPoint(masterTransform.InverseTransformPoint(OriginPosition)));
                }
                
                SpawnStrokeSegmentServerRpc(startPositions.ToArray(), endPositions.ToArray(), colorToDraw, new NetworkObjectReference(currentStrokeContainer), lastDrawingZoneIndex);
                
                spawnTimer = 0;
                lastPosition = OriginPosition;
            }
            
            wasDrawingLastFrame = true;
        }
        else
        {
            if (wasDrawingLastFrame)
            {
                 if (currentStrokeContainer != null && lastDrawingZoneIndex != -1)
                 {
                     // --- MODIFICADO: Notificar al servidor que el trazo se ha completado ---
                     CommitStrokeServerRpc(lastDrawingZoneIndex, new NetworkObjectReference(currentStrokeContainer));
                     currentStrokeContainer = null;
                     lastDrawingZoneIndex = -1;
                 }
                 
                 // Detener sonido de dibujo
                 StopDrawingSound();
            }
            wasDrawingLastFrame = false;
        }
    }

    #region Gestión de Sonido
    
    private void StartDrawingSound()
    {
        if (audioSource != null && drawingSound != null && !isSoundPlaying)
        {
            audioSource.clip = drawingSound;
            audioSource.Play();
            isSoundPlaying = true;
        }
    }
    
    private void StopDrawingSound()
    {
        if (audioSource != null && isSoundPlaying)
        {
            if (loopDrawingSound)
            {
                audioSource.Stop();
            }
            isSoundPlaying = false;
        }
    }
    
    // Método público para cambiar el volumen del sonido
    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = soundVolume;
        }
    }
    
    // Método público para cambiar el clip de sonido
    public void SetDrawingSound(AudioClip newSound)
    {
        drawingSound = newSound;
        if (audioSource != null && !isSoundPlaying)
        {
            audioSource.clip = drawingSound;
        }
    }
    
    #endregion

    #region Gestión de Estado (sin cambios)
    public void OnGrabbed() { if (IsOwner) isGrabbed = true; }
    public void OnReleased() { if (IsOwner) isGrabbed = false; }
    public void OnGesturePerformed() { if (IsOwner) isPerformingGesture = true; }
    public void OnGestureCanceled() { if (IsOwner) isPerformingGesture = false; }
    #endregion
    
    // --- ELIMINADO: La función de deshacer ya no está en el pincel ---
    // public void UndoLastStrokeInZone(int zoneIndex) { ... }

    #region Lógica de Red (RPCs)

    [ServerRpc]
    private void RequestNewStrokeContainerServerRpc(ServerRpcParams rpcParams = default)
    {
        if (strokeContainerPrefab == null) return;
        GameObject containerInstance = Instantiate(strokeContainerPrefab);
        NetworkObject containerNetObj = containerInstance.GetComponent<NetworkObject>();
        containerNetObj.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        SetCurrentStrokeContainerClientRpc(new NetworkObjectReference(containerNetObj), new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { rpcParams.Receive.SenderClientId } }
        });
    }

    [ClientRpc]
    private void SetCurrentStrokeContainerClientRpc(NetworkObjectReference containerRef, ClientRpcParams clientRpcParams = default)
    {
        if (containerRef.TryGet(out NetworkObject containerNetObj))
        {
            currentStrokeContainer = containerNetObj;
            if(strokesParent != null) currentStrokeContainer.transform.SetParent(strokesParent);
        }
    }

    // --- NUEVO: RPC para "confirmar" el trazo y registrarlo en el manager ---
    [ServerRpc]
    private void CommitStrokeServerRpc(int zoneIndex, NetworkObjectReference containerRef)
    {
        if(containerRef.TryGet(out NetworkObject containerNetObj))
        {
            DrawingZoneManager.Instance.RegisterStrokeOnServer(zoneIndex, containerNetObj);
        }
    }

    [ServerRpc]
    private void SpawnStrokeSegmentServerRpc(Vector3[] startPositions, Vector3[] endPositions, Color color, NetworkObjectReference containerRef, int masterIndex)
    {
        SpawnStrokeVisualClientRpc(startPositions, endPositions, color, containerRef, masterIndex);
    }
    
    // SpawnStrokeVisualClientRpc y RequestDestroyStrokeContainerServerRpc se quedan, ya que son necesarios para el dibujo y la destrucción
    [ClientRpc]
    private void SpawnStrokeVisualClientRpc(Vector3[] startPositions, Vector3[] endPositions, Color color, NetworkObjectReference containerRef, int masterIndex)
    {
        // ... (Este método no necesita cambios)
         if (!containerRef.TryGet(out NetworkObject containerNetObj)) return;
        if (masterIndex == -1) return;

        DrawingZoneManager.TryGetZoneObjectByIndex(masterIndex, out NetworkObject masterZoneObj);

        for (int i = 0; i < startPositions.Length; i++)
        {
            GameObject lineObj = new GameObject("StrokeSegment");
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            
            lineRenderer.positionCount = 2;
            lineRenderer.numCapVertices = 5;
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
            
            if (i > 0)
            {
                lineRenderer.material = (replicaLineMaterial != null) ? new Material(replicaLineMaterial) : new Material(lineMaterial);
                lineRenderer.generateLightingData = false;
            }
            else
            {
                lineRenderer.material = new Material(lineMaterial);
                lineRenderer.generateLightingData = true;
            }
            
            SetMaterialColor(lineRenderer.material, color);

            float currentLineWidth = this.lineWidth;

            if (i > 0) 
            {
                if (masterZoneObj != null && masterIndex < DrawingZoneManager.Instance.replicaZones.Count)
                {
                    Transform masterTransform = masterZoneObj.transform;
                    Transform replicaTransform = DrawingZoneManager.Instance.replicaZones[masterIndex].transform;
                    
                    float masterScaleX = masterTransform.lossyScale.x;
                    float replicaScaleX = replicaTransform.lossyScale.x;
                    
                    if (masterScaleX > 0.001f && replicaScaleX > 0f)
                    {
                        currentLineWidth = this.lineWidth * (replicaScaleX / masterScaleX);
                    }
                }
            }
            
            lineRenderer.startWidth = currentLineWidth;
            lineRenderer.endWidth = currentLineWidth;

            Transform parentZoneTransform = null;
            if (i == 0) 
            {
                if (masterZoneObj != null)
                {
                    parentZoneTransform = masterZoneObj.transform;
                }
            }
            else 
            {
                if (masterIndex < DrawingZoneManager.Instance.replicaZones.Count)
                {
                    parentZoneTransform = DrawingZoneManager.Instance.replicaZones[masterIndex].transform;
                }
            }
            
            if (parentZoneTransform != null)
            {
                lineObj.transform.SetParent(parentZoneTransform, false);
                lineRenderer.useWorldSpace = false;

                Vector3 localStart = parentZoneTransform.InverseTransformPoint(startPositions[i]);
                Vector3 localEnd = parentZoneTransform.InverseTransformPoint(endPositions[i]);
                
                if (i > 0)
                {
                    localStart = Quaternion.Euler(0, 0, 180) * localStart;
                    localEnd = Quaternion.Euler(0, 0, 180) * localEnd;
                }
                
                lineRenderer.SetPosition(0, localStart);
                lineRenderer.SetPosition(1, localEnd);
                
                var destroyer = lineObj.AddComponent<FollowAndDestroy>();
                destroyer.masterObject = containerNetObj;
            }
            else
            {
                Destroy(lineObj);
            }
        }
    }

    [ServerRpc]
    private void RequestDestroyStrokeContainerServerRpc(NetworkObjectReference containerToDestroyRef)
    {
        if (containerToDestroyRef.TryGet(out NetworkObject containerNetObj))
        {
            containerNetObj.Despawn(true);
        }
    }
    #endregion

    #region Utilidades (sin cambios)
    private Color GetCurrentMaterialColor()
    {
        if (lineMaterial.HasProperty("_BaseColor")) return lineMaterial.GetColor("_BaseColor");
        return lineMaterial.color;
    }

    private void SetMaterialColor(Material mat, Color color)
    {
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        else mat.SetColor("_Color", color);
    }
    #endregion
}