using Microsoft.Maui.Graphics;

namespace OOP.Lab6.Core;

public sealed class GraphDrawable : IDrawable
{
    private readonly IReadOnlyList<(int X, int Y)> _points;

    public GraphDrawable(IReadOnlyList<(int X, int Y)> points)
    {
        _points = points;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        if (_points.Count == 0)
        {
            canvas.FontColor = Colors.Gray;
            canvas.DrawString("Немає даних", dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            return;
        }

        const float margin = 40f;
        float plotLeft = margin;
        float plotBottom = dirtyRect.Height - margin;
        float plotWidth = dirtyRect.Width - margin * 2;
        float plotHeight = dirtyRect.Height - margin * 2;

        int minX = _points.Min(p => p.X);
        int maxX = _points.Max(p => p.X);
        int minY = _points.Min(p => p.Y);
        int maxY = _points.Max(p => p.Y);
        if (minX == maxX) maxX++;
        if (minY == maxY) maxY++;

        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;
        canvas.DrawLine(plotLeft, plotBottom, plotLeft + plotWidth, plotBottom);
        canvas.DrawLine(plotLeft, plotBottom, plotLeft, plotBottom - plotHeight);

        canvas.FontColor = Colors.Black;
        canvas.FontSize = 12;
        canvas.DrawString("x", plotLeft + plotWidth, plotBottom + 16, HorizontalAlignment.Center);
        canvas.DrawString("y", plotLeft - 16, plotBottom - plotHeight, HorizontalAlignment.Center);

        float MapX(int x) => plotLeft + (x - minX) / (float)(maxX - minX) * plotWidth;
        float MapY(int y) => plotBottom - (y - minY) / (float)(maxY - minY) * plotHeight;

        canvas.StrokeColor = Colors.SteelBlue;
        canvas.StrokeSize = 2;
        for (int i = 1; i < _points.Count; i++)
        {
            canvas.DrawLine(MapX(_points[i - 1].X), MapY(_points[i - 1].Y), MapX(_points[i].X), MapY(_points[i].Y));
        }

        canvas.FillColor = Colors.SteelBlue;
        foreach ((int x, int y) in _points)
            canvas.FillCircle(MapX(x), MapY(y), 4);
    }
}
