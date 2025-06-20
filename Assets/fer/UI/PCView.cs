using UnityEngine;
using UnityEngine.InputSystem;

public class PCView : MonoBehaviour
{
    [Header("Referencias Generales")]
    [Tooltip("La cámara principal que controla la vista arriba/abajo.")]
    public Transform cameraTransform;

    // --- NUEVAS VARIABLES PARA TELEPORT VR ---
    [Header("Referencias de Teletransporte VR")]
    [Tooltip("El objeto raíz de tu sistema VR (OVRCameraRig, XR Origin, etc.). Este script debería estar en este objeto.")]
    public Transform cameraRig;

    [Tooltip("El ancla de la cabeza o la cámara principal dentro del rig (CenterEyeAnchor, Main Camera, etc.).")]
    public Transform centerEye;
    // --- FIN DE NUEVAS VARIABLES ---


    [Header("Configuración de Cámara")]
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    private bool isLocked = false;
    
    private void Awake()
    {
        // Si no se asigna el cameraRig, asumimos que este script está en el objeto correcto.
        if (cameraRig == null)
        {
            cameraRig = this.transform;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (isLocked) return;
        if (cameraTransform == null) return;

        Vector2 mouseDelta = context.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
    
    public void LockControls()
    {
        isLocked = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void UnlockControls()
    {
        isLocked = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // --- LÓGICA DE TELEPORT VR MODIFICADA ---
    /// <summary>
    /// Teletransporta al jugador usando la lógica de VR y luego bloquea los controles.
    /// </summary>
    public void TeleportAndLock(Vector3 destinationPosition)
    {
        // Verificamos que las referencias para el teleport VR existan.
        if (cameraRig == null || centerEye == null)
        {
            Debug.LogError("Faltan las referencias 'cameraRig' o 'centerEye' en PCView para el teletransporte VR.");
            // Como fallback, hacemos el teletransporte simple.
            transform.position = destinationPosition;
        }
        else
        {
            // --- INICIO DE LA LÓGICA VR INTEGRADA ---
            
            // 1. Calcula el offset entre el rig y el centro de la cámara (la cabeza).
            Vector3 offset = centerEye.position - cameraRig.position;

            // 2. Mueve el rig para que el centro de la cámara coincida con el destino.
            cameraRig.position = destinationPosition - offset;

            // 3. Rota 90 grados a la derecha sobre el eje Y mundial.
            cameraRig.Rotate(0, 90f, 0, Space.World);

            // --- FIN DE LA LÓGICA VR INTEGRADA ---
        }

        // Finalmente, nos aseguramos de que los controles estén bloqueados y el cursor visible.
        LockControls();
        Debug.Log("Jugador teletransportado a " + destinationPosition + " y controles bloqueados (Modo VR).");
    }
    // --- FIN DE LA MODIFICACIÓN ---
}