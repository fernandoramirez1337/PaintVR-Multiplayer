using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public PCView pcView;
    public Transform teleportDestination;
    
    // --- INICIO DE LA MODIFICACIÓN ---
    [Tooltip("Arrastra aquí el objeto que se debe activar después del teletransporte.")]
    public GameObject objectToActivateOnTeleport;
    // --- FIN DE LA MODIFICACIÓN ---

    private VisualElement ui;
    private Button VRButton;
    private Button PCButton;
    
    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }
    
    private void OnEnable()
    {
        VRButton = ui.Q<Button>("VRButton");
        PCButton = ui.Q<Button>("PCButton");

        VRButton.clicked += OnVRButtonClicked;
        PCButton.clicked += OnPCButtonClicked;
    }
    
    private void OnVRButtonClicked()
    {
        gameObject.SetActive(false);
    }
    
    private void OnPCButtonClicked()
    {
    if (pcView != null && teleportDestination != null)
    {
      // --- INICIO DE LA MODIFICACIÓN ---
      // Ahora pasamos el objeto a activar como segundo argumento.
      pcView.Teleport(teleportDestination.position, objectToActivateOnTeleport);
      gameObject.SetActive(false);
            // --- FIN DE LA MODIFICACIÓN ---
    }
    else
    {
      Debug.LogError("Error: Asigna las referencias de 'pcView' y 'teleportDestination' en el Inspector.");
    }
    }
}