using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public sealed class RectEditor : ShapeEditor
{
    public RectEditor(ShapeStorage storage)
        : base(storage, ShapeKind.Rectangle)
    {
    }

    public override Shape CreateShape(long x1, long y1, long x2, long y2)
    {
        var shape = new RectShape();
        shape.Set(x1, y1, x2, y2);
        return shape;
    }
}
