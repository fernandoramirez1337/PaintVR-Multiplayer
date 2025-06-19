using UnityEngine;

public class SpawnUIAtPalm : MonoBehaviour
{
    [Header("Prefab Settings")]
    [Tooltip("Prefab del panel UI que se instanciará.")]
    public GameObject uiPrefab;

    [Header("Palm References")]
    [Tooltip("Transform de la palma izquierda.")]
    public Transform leftPalm;

    [Tooltip("Transform de la palma derecha.")]
    public Transform rightPalm;

    [Header("Spawn Settings")]
    [Tooltip("Distancia frente a la palma para instanciar el panel.")]
    public float spawnDistance = 0.25f;

    private GameObject currentPanel;

    /// <summary>
    /// Llama esto desde GesturePerformed() para mostrar el panel.
    /// </summary>
    /// <param name="isRightHand">True para usar la mano derecha, False para la izquierda.</param>
    public void SpawnPanel(bool isRightHand)
    {
        if (uiPrefab == null || leftPalm == null || rightPalm == null)
        {
            Debug.LogWarning("SpawnUIAtPalm: Asigna el prefab y ambos transforms de palma.");
            return;
        }

        // Limpia el panel anterior si existe
        if (currentPanel != null)
        {
            Destroy(currentPanel);
        }

        Transform selectedPalm = isRightHand ? rightPalm : leftPalm;
        Vector3 spawnPos = selectedPalm.position + selectedPalm.forward * spawnDistance;

        // Hacemos que el panel mire hacia la cámara principal
        Camera cam = Camera.main;
        Vector3 lookDirection = (cam.transform.position - spawnPos).normalized;
        Quaternion rotation = Quaternion.LookRotation(-lookDirection);

        currentPanel = Instantiate(uiPrefab, spawnPos, rotation);
    }

    /// <summary>
    /// Destruye el panel actual si existe.
    /// </summary>
    public void ClearPanel()
    {
        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
        }
    }
}