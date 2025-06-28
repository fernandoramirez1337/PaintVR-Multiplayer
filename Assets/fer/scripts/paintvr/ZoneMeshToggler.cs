// ZoneMeshToggler.cs
using UnityEngine;

public class ZoneMeshToggler : MonoBehaviour
{
    /// <summary>
    /// Connect this method to the "Gesture Performed" event in your gesture system.
    /// </summary>
    public void OnToggleGesturePerformed()
    {
        if (DrawingZoneManager.Instance != null)
        {
            DrawingZoneManager.Instance.ToggleZoneMeshRenderers();
        }
        else
        {
            Debug.LogError("DrawingZoneManager.Instance not found in the scene.");
        }
    }
}
