using UnityEngine;
using UnityEngine.UIElements;

// Nombre de archivo recomendado: TopSection.cs
[UxmlElement]
public partial class TopSection : VisualElement
{
    public TopSection()
    {
        // Establecemos el tamaño fijo para esta sección
        style.width = 1470;
        style.height = 58;

        // Registramos el método para dibujar un fondo visual
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        var painter = context.painter2D;
        painter.fillColor = new Color(56f / 255f, 56f / 255f, 56f / 255f); // Un gris oscuro
        painter.BeginPath();
        painter.MoveTo(new Vector2(0, 0));
        painter.LineTo(new Vector2(context.visualElement.contentRect.width, 0));
        painter.LineTo(new Vector2(context.visualElement.contentRect.width, context.visualElement.contentRect.height));
        painter.LineTo(new Vector2(0, context.visualElement.contentRect.height));
        painter.ClosePath();
        painter.Fill();
    }
}