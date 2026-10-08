using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;

public class EllipseShape : Shape
{
    public override ShapeKind Kind => ShapeKind.Ellipse;

    public override string DisplayName => "Еліпс";

    public override void Show(ICanvas canvas, EditorSettings settings)
    {
        var rect = NormalizeRect();
        switch (settings.EllipseDisplay)
        {
            case EllipseDisplayStyle.WhiteFill:
                canvas.FillColor = Colors.White;
                canvas.FillEllipse(rect);
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.DrawEllipse(rect);
                break;
            case EllipseDisplayStyle.ColoredFill:
                canvas.FillColor = settings.EllipseFillColor;
                canvas.FillEllipse(rect);
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.DrawEllipse(rect);
                break;
            default:
                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.DrawEllipse(rect);
                break;
        }
    }

    public void ShowCircle(ICanvas canvas, float cx, float cy, float radius, Color fill, Color stroke)
    {
        var rect = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);
        canvas.FillColor = fill;
        canvas.FillEllipse(rect);
        canvas.StrokeColor = stroke;
        canvas.StrokeSize = 2;
        canvas.DrawEllipse(rect);
    }

    protected RectF NormalizeRect()
    {
        var (x1, y1, x2, y2) = GetCoordinates();
        float left = (float)Math.Min(x1, x2);
        float top = (float)Math.Min(y1, y2);
        float right = (float)Math.Max(x1, x2);
        float bottom = (float)Math.Max(y1, y2);
        return new RectF(left, top, right - left, bottom - top);
    }
}
