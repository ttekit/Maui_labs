using Microsoft.Maui.Graphics;
using OOP.Shapes.Drawing;
using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public abstract class ShapeEditor : Editor
{
    protected ShapeEditor(ShapeStorage storage, ShapeKind kind)
    {
        Storage = storage;
        Kind = kind;
    }

    protected ShapeStorage Storage { get; }

    protected EditorSettings Settings => Storage.Settings;

    protected ShapeKind Kind { get; }

    protected float XStart { get; set; }
    protected float YStart { get; set; }
    protected float XEnd { get; set; }
    protected float YEnd { get; set; }
    protected bool IsDrawing { get; set; }

    public abstract Shape CreateShape(long x1, long y1, long x2, long y2);

    public void DrawShapes(ICanvas canvas)
    {
        foreach (Shape shape in Storage.EnumerateShapes())
            shape.Show(canvas, Settings);
    }

    public virtual void DrawRubberBand(ICanvas canvas)
    {
        if (!IsDrawing)
            return;

        RubberBandRenderer.DrawPreview(canvas, Settings, Kind, XStart, YStart, XEnd, YEnd);
    }

    public override void OnLeftButtonDown(float x, float y)
    {
        IsDrawing = true;
        XStart = XEnd = x;
        YStart = YEnd = y;
    }

    public override void OnMouseMove(float x, float y)
    {
        if (!IsDrawing)
            return;

        XEnd = x;
        YEnd = y;
    }

    public override void OnLeftButtonUp(float x, float y)
    {
        if (!IsDrawing)
            return;

        XEnd = x;
        YEnd = y;
        Shape shape = Kind is ShapeKind.Line or ShapeKind.LineWithCircles
            ? CreateShape((long)XStart, (long)YStart, (long)XEnd, (long)YEnd)
            : CreateShapeFromNormalized();
        Storage.TryAdd(shape);
        IsDrawing = false;
    }

    private Shape CreateShapeFromNormalized()
    {
        var normalized = RubberBandRenderer.NormalizeInput(Settings, Kind, XStart, YStart, XEnd, YEnd);
        return CreateShape(
            (long)normalized.Left,
            (long)normalized.Top,
            (long)normalized.Right,
            (long)normalized.Bottom);
    }
}
