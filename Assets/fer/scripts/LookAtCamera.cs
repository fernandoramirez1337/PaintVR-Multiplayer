using UnityEngine;

public class LookAtCameraFlat : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        Vector3 direction = mainCam.transform.position - transform.position;
        direction.y = 0; // Ignorar eje vertical

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(-direction); // Corregido aquí
        }
    }
}