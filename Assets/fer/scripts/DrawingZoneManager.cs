using UnityEngine;

public class DrawingZoneManager : MonoBehaviour
{
    public static DrawingZoneManager Instance { get; private set; }

    private BoxCollider currentZone;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Establece la zona activa donde se puede dibujar.
    /// </summary>
    public void SetActiveZone(BoxCollider zone)
    {
        currentZone = zone;
    }

    /// <summary>
    /// Verifica si un punto está dentro del collider, respetando rotación y escala.
    /// </summary>
    public bool IsPointInsideActiveZone(Vector3 point)
    {
        if (currentZone == null) return false;

        Vector3 closest = currentZone.ClosestPoint(point);
        return Vector3.Distance(closest, point) < 0.0001f;
    }

    /// <summary>
    /// Limpia la zona activa.
    /// </summary>
    public void ClearActiveZone()
    {
        currentZone = null;
    }

    /// <summary>
    /// Devuelve la zona activa actual.
    /// </summary>
    public BoxCollider GetCurrentZone()
    {
        return currentZone;
    }
}