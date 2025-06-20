using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LayersSection2 : VisualElement
{
    public LayersSection2()
    {
        // --- INICIO DE LA MODIFICACIÓN ---

        // 1. Establecemos un tamaño fijo para este elemento visual.
        // Esto hace que el componente tenga las dimensiones que pediste.
        style.width = 200;
        style.height = 600;

        // --- FIN DE LA MODIFICACIÓN ---

        // Mantenemos el registro del método de dibujo.
        generateVisualContent += GenerateVisualContent;
    }

    private void GenerateVisualContent(MeshGenerationContext context)
    {
        float width = contentRect.width;
        float height = contentRect.height;

        var painter = context.painter2D;
        painter.BeginPath();
        painter.lineWidth = 10f;

        // --- INICIO DE LA MODIFICACIÓN ---

        // 2. Dibujamos un rectángulo en lugar de un arco.
        // Trazamos el camino esquina por esquina.
        
        // Mueve el "lápiz" a la esquina superior izquierda (sin dibujar).
        painter.MoveTo(new Vector2(0, 0));
        
        // Dibuja una línea hasta la esquina superior derecha.
        painter.LineTo(new Vector2(width, 0));

        // Dibuja una línea hasta la esquina inferior derecha.
        painter.LineTo(new Vector2(width, height));

        // Dibuja una línea hasta la esquina inferior izquierda.
        painter.LineTo(new Vector2(0, height));
        
        // --- FIN DE LA MODIFICACIÓN ---

        // Cerramos el camino (esto dibujará la última línea de vuelta al inicio).
        painter.ClosePath();

        // Rellenamos y bordeamos la forma, igual que antes.
        painter.fillColor = Color.white;
        painter.Fill(FillRule.NonZero);
        painter.Stroke();
    }
}