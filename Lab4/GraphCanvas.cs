using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Lab4Graph;

public sealed class GraphCanvas : Control
{
    private int[,]? _matrix;
    private Point[]? _positions;
    private bool _directed;
    private string[]? _vertexLabels;

    public int[,]? Matrix
    {
        get => _matrix;
        set
        {
            _matrix = value;
            InvalidateVisual();
        }
    }

    public Point[]? Positions
    {
        get => _positions;
        set
        {
            _positions = value;
            InvalidateVisual();
        }
    }

    public bool Directed
    {
        get => _directed;
        set
        {
            _directed = value;
            InvalidateVisual();
        }
    }

    public string[]? VertexLabels
    {
        get => _vertexLabels;
        set
        {
            _vertexLabels = value;
            InvalidateVisual();
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var rect = new Rect(Bounds.Size);
        if (rect.Width < 10 || rect.Height < 10)
            return;

        context.FillRectangle(Brushes.White, rect);

        var m = _matrix;
        var pos = _positions;
        if (m == null || pos == null)
            return;

        var n = pos.Length;
        if (n == 0 || m.GetLength(0) != n)
            return;

        const double margin = 28;
        var inner = rect.Deflate(margin);
        var rVert = Math.Min(inner.Width, inner.Height) * 0.028;
        if (rVert < 2) rVert = 2;

        var pixels = new Point[n];
        for (var i = 0; i < n; i++)
            pixels[i] = ToPixel(pos[i], inner);

        var edgePen = new Pen(Brushes.Black, 1.35);
        var loopPen = new Pen(Brushes.Black, 1.2);
        var fill = new SolidColorBrush(Color.FromRgb(70, 130, 200));
        var stroke = new Pen(Brushes.DarkBlue, 1.0);

        if (_directed)
        {
            var head = Math.Max(6, rVert * 1.1);
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
            {
                if (m[i, j] != 1)
                    continue;

                if (i == j)
                    DrawSelfLoop(context, pixels[i], rVert, loopPen);
                else
                {
                    var bi = m[j, i] == 1;
                    var shift = bi ? PerpOffset(pixels[i], pixels[j], rVert * 0.9) : default;
                    var sign = bi ? (i < j ? 1.0 : -1.0) : 0.0;
                    var off = shift * sign;
                    DrawDirectedEdge(context, edgePen, pixels[i], pixels[j], rVert, head, off);
                }
            }
        }
        else
        {
            for (var i = 0; i < n; i++)
            for (var j = i + 1; j < n; j++)
            {
                if (m[i, j] != 1)
                    continue;

                var pi = ClipToCircle(pixels[i], pixels[j], rVert);
                var pj = ClipToCircle(pixels[j], pixels[i], rVert);
                context.DrawLine(edgePen, pi, pj);
            }

            for (var i = 0; i < n; i++)
            {
                if (m[i, i] != 1)
                    continue;
                DrawSelfLoop(context, pixels[i], rVert, loopPen);
            }
        }

        var labels = _vertexLabels;
        for (var i = 0; i < n; i++)
        {
            var c = pixels[i];
            var vr = VertexCircleRadius(labels, i, rVert);
            var vrGeom = new EllipseGeometry(new Rect(c.X - vr, c.Y - vr, 2 * vr, 2 * vr));
            context.DrawGeometry(fill, stroke, vrGeom);

            var text = labels != null && i < labels.Length ? labels[i] : (i + 1).ToString();
            var fontSize = Math.Max(8, Math.Min(rVert * 1.0, 11));
            if (text.Length > 6)
                fontSize = Math.Max(7, fontSize * 0.75);
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.Default),
                fontSize,
                Brushes.White);
            context.DrawText(ft, new Point(c.X - ft.Width / 2, c.Y - ft.Height / 2));
        }
    }

    private static double VertexCircleRadius(string[]? labels, int i, double rVert)
    {
        if (labels == null || i >= labels.Length)
            return rVert;
        var len = labels[i].Length;
        return len <= 1 ? rVert : Math.Min(rVert * 2.2, rVert + len * 2.8);
    }

    private static Point ToPixel(Point norm, Rect inner) =>
        new(inner.X + norm.X * inner.Width, inner.Y + norm.Y * inner.Height);

    private static Vector PerpOffset(Point from, Point to, double scale)
    {
        var v = new Vector(to.X - from.X, to.Y - from.Y);
        var lenSq = v.X * v.X + v.Y * v.Y;
        if (lenSq < 1e-24)
            return new Vector(0, 0);
        v /= Math.Sqrt(lenSq);
        return new Vector(-v.Y, v.X) * scale;
    }

    private static Point ClipToCircle(Point from, Point toward, double r)
    {
        var dir = new Vector(toward.X - from.X, toward.Y - from.Y);
        var len = dir.Length;
        if (len < 1e-9)
            return from;
        dir /= len;
        return from + dir * r;
    }

    private static void DrawDirectedEdge(
        DrawingContext ctx,
        Pen pen,
        Point from,
        Point to,
        double rVert,
        double headLen,
        Vector parallelOffset)
    {
        var f = from + parallelOffset;
        var t = to + parallelOffset;
        var dir = new Vector(t.X - f.X, t.Y - f.Y);
        var len = dir.Length;
        if (len < 1e-9)
            return;
        dir /= len;
        var vrFrom = rVert;
        var vrTo = rVert;
        f = f + dir * vrFrom;
        t = t - dir * (vrTo + headLen * 0.35);

        ctx.DrawLine(pen, f, t);

        var basePt = t;
        var back = basePt - dir * headLen;
        var perp = new Vector(-dir.Y, dir.X) * (headLen * 0.35);
        var geo = new StreamGeometry();
        using (var s = geo.Open())
        {
            s.BeginFigure(basePt, true);
            s.LineTo(back + perp);
            s.LineTo(back - perp);
            s.EndFigure(true);
        }

        ctx.DrawGeometry(Brushes.Black, null, geo);
    }

    private static void DrawSelfLoop(DrawingContext ctx, Point center, double rVert, Pen pen)
    {
        var geo = new StreamGeometry();
        using (var s = geo.Open())
        {
            var p0 = center + new Vector(rVert * 0.85, -rVert * 0.15);
            var p1 = center + new Vector(rVert * 2.0, -rVert * 2.5);
            var p2 = center + new Vector(-rVert * 2.0, -rVert * 2.5);
            var p3 = center + new Vector(-rVert * 0.85, -rVert * 0.15);
            s.BeginFigure(p0, false);
            s.CubicBezierTo(p1, p2, p3);
            s.EndFigure(false);
        }

        ctx.DrawGeometry(null, pen, geo);
    }
}
