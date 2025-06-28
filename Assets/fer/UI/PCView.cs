using UnityEngine;
using UnityEngine.InputSystem;

public class PCView : MonoBehaviour
{
    [Header("Referencias")]
    public Transform cameraTransform;
    public Transform cameraRig;
    public Transform centerEye;

    [Header("Configuración de Cámara")]
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;
    
    private void Awake()
    {
        if (cameraRig == null) cameraRig = this.transform;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (cameraTransform == null) return;
        
        Vector2 mouseDelta = context.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // --- INICIO DE LA MODIFICACIÓN ---
    /// <summary>
    /// Teletransporta al jugador y activa un objeto especificado.
    /// </summary>
    /// <param name="destinationPosition">La posición a la que se moverá el jugador.</param>
    /// <param name="objectToActivate">El GameObject que se activará después del teletransporte.</param>
    public void Teleport(Vector3 destinationPosition, GameObject objectToActivate)
    {
        // 1. Lógica de teletransporte (sin cambios)
        if (cameraRig != null && centerEye != null)
        {
            Vector3 offset = centerEye.position - cameraRig.position;
            cameraRig.position = destinationPosition - offset;
            cameraRig.Rotate(0, 90f, 0, Space.World);
        }
        else
        {
            transform.position = destinationPosition;
        }
        Debug.Log("Jugador teletransportado a " + destinationPosition);

        // 2. Nueva lógica para activar el objeto
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log("Objeto '" + objectToActivate.name + "' activado.");
        }
    }
    // --- FIN DE LA MODIFICACIÓN ---
}