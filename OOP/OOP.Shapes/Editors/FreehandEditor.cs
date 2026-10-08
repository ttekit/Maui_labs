using Microsoft.Maui.Graphics;
using OOP.Shapes.Shapes;

namespace OOP.Shapes.Editors;

public sealed class FreehandEditor : ShapeEditor
{
    private const float MinSegmentLengthSq = 4f;
    private FreehandShape? _activeStroke;

    public FreehandEditor(ShapeStorage storage)
        : base(storage, ShapeKind.Freehand)
    {
    }

    public override Shape CreateShape(long x1, long y1, long x2, long y2)
    {
        var shape = new FreehandShape();
        shape.Set(x1, y1, x2, y2);
        return shape;
    }

    public override void DrawRubberBand(ICanvas canvas)
    {
    }

    public override void OnLeftButtonDown(float x, float y)
    {
        IsDrawing = true;
        _activeStroke = new FreehandShape();
        if (!_activeStroke.TryAddPoint(x, y, 0f))
            _activeStroke = null;

        if (_activeStroke is not null && !Storage.TryAdd(_activeStroke))
            _activeStroke = null;
    }

    public override void OnMouseMove(float x, float y)
    {
        if (!IsDrawing || _activeStroke is null)
            return;

        _activeStroke.TryAddPoint(x, y, MinSegmentLengthSq);
    }

    public override void OnLeftButtonUp(float x, float y)
    {
        if (!IsDrawing)
            return;

        _activeStroke?.TryAddPoint(x, y, MinSegmentLengthSq);
        _activeStroke = null;
        IsDrawing = false;
    }
}
