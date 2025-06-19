using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class UIButtonColorSetter : MonoBehaviour
{
    [Header("Configuración")]
    public Color buttonColor;
    public Material materialToChange; // Material a aplicar al spray o similar

    private Image targetImage;
    private Toggle toggle;
    private Material runtimeMaterialInstance;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();

        // Buscar el Image en los descendientes
        targetImage = GetComponentInChildren<Image>();

        if (targetImage != null)
        {
            // Crear una instancia del material para no afectar a todos
            runtimeMaterialInstance = new Material(targetImage.material);
            targetImage.material = runtimeMaterialInstance;
            runtimeMaterialInstance.SetColor("_Color", buttonColor);
        }

        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

public void OnToggleChanged(bool isOn)
{
    if (isOn && materialToChange != null)
    {
        if (materialToChange.HasProperty("_BaseColor"))
        {
            materialToChange.SetColor("_BaseColor", buttonColor);
        }
        else if (materialToChange.HasProperty("_Color"))
        {
            materialToChange.SetColor("_Color", buttonColor);
        }
        else
        {
            Debug.LogWarning("Material does not have a supported color property.");
        }
    }
}
}