using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMenuController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí el objeto del Jugador que tiene el script PCView.")]
    public PCView pcView;

    [Tooltip("Arrastra un objeto para marcar el punto de teletransporte.")]
    public Transform teleportDestination;

    // --- VARIABLES DE UI ---
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

    /// <summary>
    /// REQUISITO 1: Desaparecer cuando se aprieta el botón VRButton.
    /// </summary>
    private void OnVRButtonClicked()
    {
        Debug.Log("PlayerMenuController: Botón VR presionado. Ocultando este GameObject.");
        gameObject.SetActive(false);
    }

    /// <summary>
    /// REQUISITO 2: Debe llamar a PCView cuando se aprieta el botón PCButton.
    /// </summary>
    private void OnPCButtonClicked()
    {
    if (pcView != null && teleportDestination != null)
    {
      Debug.Log("PlayerMenuController: Botón PC presionado. Llamando a PCView.TeleportAndLock().");
      pcView.TeleportAndLock(teleportDestination.position);
      gameObject.SetActive(false);
    }
    else
    {
      Debug.LogError("Error: Asigna las referencias de 'pcView' y 'teleportDestination' en el Inspector.");
    }
    }
}