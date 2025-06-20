using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
  // --- INICIO DE LA MODIFICACIÓN ---
  // Añadimos una variable pública para mantener una referencia a la segunda UI.
  // Puedes arrastrar el GameObject de tu segunda UI a este campo en el Inspector de Unity.
  public GameObject secondUI;
  // --- FIN DE LA MODIFICACIÓN ---

  public VisualElement ui;

  public Button playButton;
  public Button settingsButton;
  public Button quitButton;

  private void Awake()
  {
    ui = GetComponent<UIDocument>().rootVisualElement;
    
    // Es una buena práctica asegurarse de que la segunda UI esté desactivada al inicio.
    if (secondUI != null)
    {
      secondUI.SetActive(false);
    }
  }

  private void OnEnable()
  {
    playButton = ui.Q<Button>("PlayButton");
    settingsButton = ui.Q<Button>("SettingsButton");
    quitButton = ui.Q<Button>("QuitButton");

    playButton.clicked += OnPlayButtonClicked;
    settingsButton.clicked += OnSettingsButtonClicked;
    quitButton.clicked += OnQuitButtonClicked;
  }

  private void OnQuitButtonClicked()
  {
    Application.Quit();
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
  }
  private void OnSettingsButtonClicked()
  {
    Debug.Log("Settings button clicked");
  }

  private void OnPlayButtonClicked()
  {
    // --- INICIO DE LA MODIFICACIÓN ---

    // Primero, verificamos que la segunda UI ha sido asignada para evitar errores.
    if (secondUI != null)
    {
      // Activamos el GameObject de la segunda UI.
      secondUI.SetActive(true);
    }
    else
    {
      // Si te olvidas de asignarla, este mensaje te lo recordará.
      Debug.LogWarning("No se ha asignado una segunda UI en el MainMenuController.");
    }
    
    // Finalmente, desactivamos el GameObject de este menú principal para que desaparezca.
    gameObject.SetActive(false);
    
    // --- FIN DE LA MODIFICACIÓN ---
  }
}