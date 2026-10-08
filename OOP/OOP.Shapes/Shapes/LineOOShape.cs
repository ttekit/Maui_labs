using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;


public sealed class LineOOShape : LineShape
{
    private readonly EllipseShape _ellipseHelper = new();

    public override ShapeKind Kind => ShapeKind.LineWithCircles;

    public override string DisplayName => "Лінія з кружечками";

    public override void Show(ICanvas canvas, EditorSettings settings)
    {
        base.Show(canvas, settings);
        var (x1, y1, x2, y2) = GetCoordinates();
        const float radius = 6f;
        _ellipseHelper.Set(x1, y1, x1, y1);
        _ellipseHelper.ShowCircle(canvas, (float)x1, (float)y1, radius, Colors.LightBlue, Colors.Black);
        _ellipseHelper.Set(x2, y2, x2, y2);
        _ellipseHelper.ShowCircle(canvas, (float)x2, (float)y2, radius, Colors.LightBlue, Colors.Black);
    }
}
