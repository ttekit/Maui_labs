using Microsoft.Maui.Graphics;

namespace OOP.Shapes.Drawing;

public static class CubeWireframe
{
    private const float DepthFactor = 0.3f;
    private const float MinDepth = 8f;

    public static float ComputeDepth(float width, float height)
    {
        if (width <= 0f || height <= 0f)
            return MinDepth;

        return Math.Max(MinDepth, Math.Min(width, height) * DepthFactor);
    }

    public static void Draw(
        ICanvas canvas,
        float left,
        float top,
        float right,
        float bottom,
        Color strokeColor,
        float strokeSize = 2f,
        float[]? dashPattern = null)
    {
        float width = right - left;
        float height = bottom - top;
        if (width <= 0f || height <= 0f)
            return;

        float depth = ComputeDepth(width, height);
        float backLeft = left + depth;
        float backTop = top - depth;
        float backRight = right + depth;
        float backBottom = bottom - depth;

        canvas.StrokeColor = strokeColor;
        canvas.StrokeSize = strokeSize;
        canvas.StrokeDashPattern = dashPattern ?? Array.Empty<float>();

        DrawEdge(canvas, left, top, right, top);
        DrawEdge(canvas, right, top, right, bottom);
        DrawEdge(canvas, right, bottom, left, bottom);
        DrawEdge(canvas, left, bottom, left, top);

        DrawEdge(canvas, backLeft, backTop, backRight, backTop);
        DrawEdge(canvas, backRight, backTop, backRight, backBottom);
        DrawEdge(canvas, backRight, backBottom, backLeft, backBottom);
        DrawEdge(canvas, backLeft, backBottom, backLeft, backTop);

        DrawEdge(canvas, left, top, backLeft, backTop);
        DrawEdge(canvas, right, top, backRight, backTop);
        DrawEdge(canvas, right, bottom, backRight, backBottom);
        DrawEdge(canvas, left, bottom, backLeft, backBottom);

        canvas.StrokeDashPattern = Array.Empty<float>();
    }

    public static (float Left, float Top, float Right, float Bottom) NormalizeFace(
        float x1,
        float y1,
        float x2,
        float y2)
    {
        float left = Math.Min(x1, x2);
        float top = Math.Min(y1, y2);
        float right = Math.Max(x1, x2);
        float bottom = Math.Max(y1, y2);
        return (left, top, right, bottom);
    }

    private static void DrawEdge(ICanvas canvas, float x1, float y1, float x2, float y2)
    {
        canvas.DrawLine(x1, y1, x2, y2);
    }
}
