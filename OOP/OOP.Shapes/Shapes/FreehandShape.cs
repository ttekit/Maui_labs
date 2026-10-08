using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;


public sealed class FreehandShape : Shape
{
    private readonly List<(float X, float Y)> _points = new();

    public override ShapeKind Kind => ShapeKind.Freehand;

    public override string DisplayName => "Малювання";

    public bool TryAddPoint(float x, float y, float minDistanceSq)
    {
        if (_points.Count > 0)
        {
            (float lastX, float lastY) = _points[^1];
            float dx = x - lastX;
            float dy = y - lastY;
            if (dx * dx + dy * dy < minDistanceSq)
                return false;
        }

        _points.Add((x, y));
        UpdateBounds();
        return true;
    }

    public override void Show(ICanvas canvas, EditorSettings settings)
    {
        if (_points.Count == 0)
            return;

        if (_points.Count == 1)
        {
            (float x, float y) = _points[0];
            canvas.FillColor = Colors.Black;
            canvas.FillCircle(x, y, 3f);
            return;
        }

        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;
        for (int i = 1; i < _points.Count; i++)
        {
            (float x1, float y1) = _points[i - 1];
            (float x2, float y2) = _points[i];
            canvas.DrawLine(x1, y1, x2, y2);
        }
    }

    private void UpdateBounds()
    {
        float minX = _points.Min(point => point.X);
        float minY = _points.Min(point => point.Y);
        float maxX = _points.Max(point => point.X);
        float maxY = _points.Max(point => point.Y);
        Set((long)minX, (long)minY, (long)maxX, (long)maxY);
    }
}
