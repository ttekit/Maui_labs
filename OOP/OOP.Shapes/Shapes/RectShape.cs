using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;

public class RectShape : Shape
{
    public override ShapeKind Kind => ShapeKind.Rectangle;

    public override string DisplayName => "Прямокутник";

    public override void Show(ICanvas canvas, EditorSettings settings, bool isSelected = false)
    {
        var rect = NormalizeRect();
        Color strokeColor = GetStrokeColor(isSelected);
        switch (settings.RectDisplay)
        {
            case RectDisplayStyle.WhiteFill:
                canvas.FillColor = Colors.White;
                canvas.FillRectangle(rect);
                canvas.StrokeColor = strokeColor;
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(rect);
                break;
            case RectDisplayStyle.ColoredFill:
                canvas.FillColor = settings.RectFillColor;
                canvas.FillRectangle(rect);
                canvas.StrokeColor = strokeColor;
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(rect);
                break;
            default:
                canvas.StrokeColor = strokeColor;
                canvas.StrokeSize = 2;
                canvas.DrawRectangle(rect);
                break;
        }
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
