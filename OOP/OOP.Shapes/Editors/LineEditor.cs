using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public class LineEditor : ShapeEditor
{
    public LineEditor(ShapeStorage storage, ShapeKind kind)
        : base(storage, kind)
    {
    }

    public override Shape CreateShape(long x1, long y1, long x2, long y2)
    {
        var shape = CreateLineShape();
        shape.Set(x1, y1, x2, y2);
        return shape;
    }

    protected virtual Shape CreateLineShape() => new LineShape();
}

public sealed class LineOOEditor : LineEditor
{
    public LineOOEditor(ShapeStorage storage)
        : base(storage, ShapeKind.LineWithCircles)
    {
    }

    protected override Shape CreateLineShape() => new LineOOShape();
}

public sealed class CubeEditor : LineEditor
{
    public CubeEditor(ShapeStorage storage)
        : base(storage, ShapeKind.Cube)
    {
    }

    protected override Shape CreateLineShape() => new CubeShape();
}
