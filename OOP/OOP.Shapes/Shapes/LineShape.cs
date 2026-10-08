using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;

public class LineShape : Shape
{
    public override ShapeKind Kind => ShapeKind.Line;

    public override string DisplayName => "Лінія";

    public override void Show(ICanvas canvas, EditorSettings settings, bool isSelected = false)
    {
        var (x1, y1, x2, y2) = GetCoordinates();
        canvas.StrokeColor = GetStrokeColor(isSelected);
        canvas.StrokeSize = 2;
        canvas.DrawLine((float)x1, (float)y1, (float)x2, (float)y2);
    }

    public void ShowSegment(ICanvas canvas, float x1, float y1, float x2, float y2, Color color, float strokeSize = 2f)
    {
        canvas.StrokeColor = color;
        canvas.StrokeSize = strokeSize;
        canvas.DrawLine(x1, y1, x2, y2);
    }
}
