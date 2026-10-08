using Microsoft.Maui.Graphics;
using OOP.Shapes.Drawing;

namespace OOP.Shapes.Shapes;


public sealed class CubeShape : LineShape
{
    public override ShapeKind Kind => ShapeKind.Cube;

    public override string DisplayName => "Каркас куба";

    public override void Show(ICanvas canvas, EditorSettings settings)
    {
        var (x1, y1, x2, y2) = GetCoordinates();
        var (left, top, right, bottom) = CubeWireframe.NormalizeFace((float)x1, (float)y1, (float)x2, (float)y2);
        CubeWireframe.Draw(canvas, left, top, right, bottom, Colors.Black);
    }
}
