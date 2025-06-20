using UnityEngine;
using UnityEngine.UIElements;

// Nombre de archivo recomendado: ProjectSection.cs
[UxmlElement]
public partial class ProjectSection : VisualElement
{
    public ProjectSection()
    {
        // Establecemos el tamaño fijo para esta sección
        style.width = 1118;
        style.height = 344;

        // Registramos el método para dibujar un fondo visual
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        var painter = context.painter2D;
        painter.fillColor = new Color(0.8f, 0.8f, 0.8f); // Un gris claro
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