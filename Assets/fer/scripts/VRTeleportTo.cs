using UnityEngine;

public class VRTeleportTo : MonoBehaviour
{
    public Transform cameraRig;     // OVRCameraRig o XR Origin
    public Transform centerEye;     // CenterEyeAnchor o Main Camera
    public Transform destination;   // Punto exacto donde quieres que esté la cabeza del jugador

    void Start()
    {
        if (cameraRig == null || centerEye == null || destination == null)
        {
            Debug.LogWarning("TeleportTo: Faltan referencias.");
            return;
        }

        // --- POSICIÓN ---
        // Calcula el offset entre el rig y el centro de la cámara
        Vector3 offset = centerEye.position - cameraRig.position;

        // Mueve el rig para que el centro de la cámara coincida con el destino
        cameraRig.position = destination.position - offset;

        // --- ROTACIÓN ---
        // Rota 90° a la derecha (eje Y)
        cameraRig.Rotate(0, 90f, 0, Space.World);
    }
}