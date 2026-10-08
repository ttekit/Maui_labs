using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public sealed class PointEditor : ShapeEditor
{
    public PointEditor(ShapeStorage storage)
        : base(storage, ShapeKind.Point)
    {
    }

    public override Shape CreateShape(long x1, long y1, long x2, long y2)
    {
        var shape = new PointShape();
        shape.Set(x1, y1, x1, y1);
        return shape;
    }

    public override void OnLeftButtonUp(float x, float y)
    {
        if (!IsDrawing)
            return;

        var shape = new PointShape();
        shape.Set((long)x, (long)y, (long)x, (long)y);
        Storage.TryAdd(shape);
        IsDrawing = false;
    }
}
