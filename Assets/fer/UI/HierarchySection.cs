using UnityEngine;
using UnityEngine.UIElements;

// Nombre de archivo recomendado: HierarchySection.cs
[UxmlElement]
public partial class HierarchySection : VisualElement
{
    public HierarchySection()
    {
        // Establecemos el tamaño fijo para esta sección
        style.width = 276;
        style.height = 491;

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
        painter.LineTo(new Vector2(rect.xMax, rect.y));
        painter.LineTo(new Vector2(rect.xMax, rect.yMax));
        painter.LineTo(new Vector2(rect.x, rect.yMax));
        painter.ClosePath();
        painter.Fill();
    }
}