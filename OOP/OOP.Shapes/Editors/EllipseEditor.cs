using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public sealed class EllipseEditor : ShapeEditor
{
    public EllipseEditor(ShapeStorage storage)
        : base(storage, ShapeKind.Ellipse)
    {
    }

    public override Shape CreateShape(long x1, long y1, long x2, long y2)
    {
        var shape = new EllipseShape();
        shape.Set(x1, y1, x2, y2);
        return shape;
    }
}
