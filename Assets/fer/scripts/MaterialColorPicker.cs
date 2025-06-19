using UnityEngine;

public class MaterialColorPicker : MonoBehaviour
{
    [Header("Renderer and Material")]
    [Tooltip("Renderer of the object whose material you want to change.")]
    public Renderer targetRenderer;
    [Tooltip("If the Renderer has multiple materials, index of the one to modify.")]
    public int materialIndex = 0;

    // Stores the initial base color before any changes
    private Color initialBaseColor;
    // Stores the initial emission color before any changes
    private Color initialEmissionColor;
    // Tracks whether emission was enabled initially
    private bool initialEmissionEnabled;

    private void Awake()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("MaterialColorPicker: No Renderer assigned in Awake.");
            return;
        }
        var materials = targetRenderer.materials;
        Material mat = materials[materialIndex];
        // Capture initial base color
        if (mat.HasProperty("_BaseColor"))
            initialBaseColor = mat.GetColor("_BaseColor");
        else if (mat.HasProperty("_Color"))
            initialBaseColor = mat.GetColor("_Color");
        // Capture initial emission state and color
        if (mat.HasProperty("_EmissionColor"))
        {
            initialEmissionEnabled = mat.IsKeywordEnabled("_EMISSION");
            initialEmissionColor = mat.GetColor("_EmissionColor");
        }
        else
        {
            initialEmissionEnabled = false;
            initialEmissionColor = Color.black;
        }
    }

    [Header("Palette of Colors")]
    [Tooltip("List of colors to choose from. Must contain at least 1 color.")]
    public Color[] paintColors = new Color[10]
    {
        Color.red, Color.green, Color.blue, Color.yellow, Color.magenta,
        Color.cyan, Color.gray, Color.white, new Color(1f, 0.5f, 0f), new Color(0.5f, 0f, 1f)
    };

    /// <summary>
    /// Call from your gesture callback, passing an index (0 to paintColors.Length-1) 
    /// to select the color to paint the material.
    /// </summary>
    /// <param name="colorIndex">Index of the color in paintColors.</param>
    public void PaintColor(int colorIndex)
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("MaterialColorPicker: No Renderer assigned.");
            return;
        }
        if (paintColors == null || paintColors.Length == 0)
        {
            Debug.LogWarning("MaterialColorPicker: paintColors array is empty.");
            return;
        }

        // Clamp index to valid range
        int idx = Mathf.Clamp(colorIndex, 0, paintColors.Length - 1);
        Color chosenColor = paintColors[idx];

        // Get a unique instance of the material
        var materials = targetRenderer.materials;
        Material mat = materials[materialIndex];

        // Apply to base color (_BaseColor for URP/HDRP or _Color for Built-in)
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", chosenColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", chosenColor);

        // Apply to emission if supported
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", chosenColor);
        }

        // Reassign materials array to update the instance
        materials[materialIndex] = mat;
        targetRenderer.materials = materials;
    }
    /// <summary>
    /// Reverts the material's color and emission back to the previous state.
    /// </summary>
    public void RevertColor()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("MaterialColorPicker: No Renderer assigned for revert.");
            return;
        }
        var materials = targetRenderer.materials;
        Material mat = materials[materialIndex];
        // Revert base color
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", initialBaseColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", initialBaseColor);
        // Revert emission
        if (mat.HasProperty("_EmissionColor"))
        {
            if (initialEmissionEnabled)
                mat.EnableKeyword("_EMISSION");
            else
                mat.DisableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", initialEmissionColor);
        }
        materials[materialIndex] = mat;
        targetRenderer.materials = materials;
    }
}
