using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Shapes;

public sealed class PointShape : Shape
{
    public override ShapeKind Kind => ShapeKind.Point;

    public override string DisplayName => "Крапка";

    public override void Show(ICanvas canvas, EditorSettings settings)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillCircle((float)GetCoordinates().X1, (float)GetCoordinates().Y1, 3f);
    }
}
