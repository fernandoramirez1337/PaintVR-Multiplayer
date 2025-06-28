using UnityEngine;
using UnityEngine.UIElements;

// Nombre de archivo recomendado: InspectorSection.cs
[UxmlElement]
public partial class InspectorSection : VisualElement
{
    public InspectorSection()
    {
        // Establecemos el tamaño fijo para esta sección
        style.width = 355;
        style.height = 838;

        // Registramos el método para dibujar un fondo visual
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        var painter = context.painter2D;
        painter.fillColor = new Color(56f / 255f, 56f / 255f, 56f / 255f); // Un gris oscuro
        var rect = context.visualElement.contentRect;
        painter.BeginPath();
        painter.MoveTo(new Vector2(rect.x, rect.y));
        painter.LineTo(new Vector2(rect.x + rect.width, rect.y));
        painter.LineTo(new Vector2(rect.x + rect.width, rect.y + rect.height));
        painter.LineTo(new Vector2(rect.x, rect.y + rect.height));
        painter.ClosePath();
        painter.Fill();
    }
}