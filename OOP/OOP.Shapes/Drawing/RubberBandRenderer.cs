using Microsoft.Maui.Graphics;
using OOP.Shapes.Shapes;

namespace OOP.Shapes.Drawing;

public static class RubberBandRenderer
{
    public static void DrawPreview(
        ICanvas canvas,
        EditorSettings settings,
        ShapeKind kind,
        float x1,
        float y1,
        float x2,
        float y2)
    {
        canvas.StrokeColor = settings.RubberBandColor;
        canvas.StrokeSize = 2;
        canvas.StrokeDashPattern = settings.RubberBandDashed ? new float[] { 4, 4 } : Array.Empty<float>();

        switch (kind)
        {
            case ShapeKind.Point:
                canvas.DrawCircle(x1, y1, 3);
                break;
            case ShapeKind.Line:
            case ShapeKind.LineWithCircles:
                canvas.DrawLine(x1, y1, x2, y2);
                break;
            case ShapeKind.Rectangle:
                DrawRectPreview(canvas, settings, x1, y1, x2, y2);
                break;
            case ShapeKind.Cube:
                DrawCubePreview(canvas, settings, x1, y1, x2, y2);
                break;
            case ShapeKind.Ellipse:
                DrawEllipsePreview(canvas, settings, x1, y1, x2, y2);
                break;
        }

        canvas.StrokeDashPattern = Array.Empty<float>();
    }

    private static void DrawCubePreview(ICanvas canvas, EditorSettings settings, float x1, float y1, float x2, float y2)
    {
        var (left, top, right, bottom) = CubeWireframe.NormalizeFace(x1, y1, x2, y2);
        float[]? dash = settings.RubberBandDashed ? new float[] { 4, 4 } : null;
        CubeWireframe.Draw(canvas, left, top, right, bottom, settings.RubberBandColor, 2f, dash);
    }

    private static void DrawRectPreview(ICanvas canvas, EditorSettings settings, float x1, float y1, float x2, float y2)
    {
        var (left, top, right, bottom) = NormalizeInput(settings, ShapeKind.Rectangle, x1, y1, x2, y2);
        canvas.DrawRectangle(left, top, right - left, bottom - top);
    }

    private static void DrawEllipsePreview(ICanvas canvas, EditorSettings settings, float x1, float y1, float x2, float y2)
    {
        var (left, top, right, bottom) = NormalizeInput(settings, ShapeKind.Ellipse, x1, y1, x2, y2);
        canvas.DrawEllipse(left, top, right - left, bottom - top);
    }

    public static (float Left, float Top, float Right, float Bottom) NormalizeInput(
        EditorSettings settings,
        ShapeKind kind,
        float x1,
        float y1,
        float x2,
        float y2)
    {
        if (kind == ShapeKind.Rectangle && settings.RectInput == RectInputMode.CenterToCorner)
            return CenterToCorner(x1, y1, x2, y2);

        if (kind == ShapeKind.Ellipse && settings.EllipseInput == EllipseInputMode.CenterToCorner)
            return CenterToCorner(x1, y1, x2, y2);

        float left = Math.Min(x1, x2);
        float top = Math.Min(y1, y2);
        float right = Math.Max(x1, x2);
        float bottom = Math.Max(y1, y2);
        return (left, top, right, bottom);
    }

    private static (float, float, float, float) CenterToCorner(float cx, float cy, float x2, float y2)
    {
        float dx = x2 - cx;
        float dy = y2 - cy;
        return (cx - dx, cy - dy, cx + dx, cy + dy);
    }
}
