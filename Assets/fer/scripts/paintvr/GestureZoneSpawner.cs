// GestureZoneSpawner.cs
using UnityEngine;

public class GestureZoneSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    [Tooltip("El punto de origen (ej. la palma de la mano) donde se calculará la posición de la zona.")]
    [SerializeField] private Transform spawnPoint;

    [Header("Offset Local")]
    [Tooltip("Distancia de desplazamiento hacia la derecha/izquierda de la palma.")]
    [SerializeField] private float offsetX = 0f;

    [Tooltip("Distancia de desplazamiento hacia adelante/atrás de la palma.")]
    [SerializeField] private float offsetZ = 0.15f;

    public void OnGesturePerformed()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("¡No se ha asignado un Spawn Point!");
            return;
        }

        Vector3 positionOffset = (spawnPoint.right * offsetX) + (spawnPoint.forward * offsetZ);
        Vector3 finalSpawnPosition = spawnPoint.position + positionOffset;
        Quaternion finalSpawnRotation = spawnPoint.rotation;

        if (DrawingZoneManager.Instance != null)
        {
            DrawingZoneManager.Instance.RequestSpawnZone(finalSpawnPosition, finalSpawnRotation);
        }
        else
        {
            Debug.LogError("DrawingZoneManager.Instance no encontrado en la escena.");
        }
    }
}