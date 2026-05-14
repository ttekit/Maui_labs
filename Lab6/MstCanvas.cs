using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Lab6Graph;

public sealed class MstCanvas : Control
{
    private int[,]? _w;
    private Point[]? _positions;
    private HashSet<(int U, int V)>? _mstEdges;
    private (int U, int V)? _candidateEdge;

    public int[,]? WeightMatrix
    {
        get => _w;
        set
        {
            _w = value;
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

    public HashSet<(int U, int V)>? MstEdges
    {
        get => _mstEdges;
        set
        {
            _mstEdges = value;
            InvalidateVisual();
        }
    }

    public (int U, int V)? CandidateEdge
    {
        get => _candidateEdge;
        set
        {
            _candidateEdge = value;
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

        var w = _w;
        var pos = _positions;
        if (w == null || pos == null)
            return;

        var n = pos.Length;
        if (n == 0 || w.GetLength(0) != n)
            return;

        var mst = _mstEdges ?? new HashSet<(int, int)>();
        var cand = _candidateEdge;

        const double margin = 28;
        var inner = rect.Deflate(margin);
        var rVert = Math.Min(inner.Width, inner.Height) * 0.028;
        if (rVert < 2) rVert = 2;

        var pixels = new Point[n];
        for (var i = 0; i < n; i++)
            pixels[i] = ToPixel(pos[i], inner);

        var penBase = new Pen(new SolidColorBrush(Color.FromRgb(170, 170, 175)), 1.15);
        var penMst = new Pen(new SolidColorBrush(Color.FromRgb(0, 130, 55)), 2.75);
        var penCandBrush = new SolidColorBrush(Color.FromRgb(210, 110, 0));
        var penCand = new Pen(penCandBrush, 2.4, DashStyle.Dash);

        for (var i = 0; i < n; i++)
        for (var j = i; j < n; j++)
        {
            var wt = w[i, j];
            if (wt <= 0)
                continue;

            var edge = (i, j);
            var inMst = mst.Contains(edge);
            var isCand = cand == edge;

            Pen pen;
            IBrush labelBrush;
            if (isCand)
            {
                pen = penCand;
                labelBrush = penCandBrush;
            }
            else if (inMst)
            {
                pen = penMst;
                labelBrush = penMst.Brush ?? Brushes.DarkGreen;
            }
            else
            {
                pen = penBase;
                labelBrush = penBase.Brush ?? Brushes.DimGray;
            }

                if (i == j)
                {
                    DrawSelfLoop(context, pixels[i], rVert, pen);
                    var ft = new FormattedText(
                        wt.ToString(),
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(FontFamily.Default),
                        10,
                        labelBrush);
                    var c = pixels[i] + new Vector(0, -rVert * 3.2);
                    context.DrawText(ft, new Point(c.X - ft.Width / 2, c.Y - ft.Height / 2));
                }
                else
                {
                    var pi = ClipToCircle(pixels[i], pixels[j], rVert);
                    var pj = ClipToCircle(pixels[j], pixels[i], rVert);
                    context.DrawLine(pen, pi, pj);
                    DrawWeightLabel(context, pi, pj, wt.ToString(), labelBrush);
                }
        }

        for (var i = 0; i < n; i++)
        {
            var c = pixels[i];
            var fill = new SolidColorBrush(Color.FromRgb(70, 130, 200));
            var stroke = new Pen(Brushes.DarkBlue, 1.0);
            var vrGeom = new EllipseGeometry(new Rect(c.X - rVert, c.Y - rVert, 2 * rVert, 2 * rVert));
            context.DrawGeometry(fill, stroke, vrGeom);

            var label = (i + 1).ToString();
            var ft = new FormattedText(
                label,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.Default),
                Math.Max(10, rVert * 1.1),
                Brushes.White);
            context.DrawText(ft, new Point(c.X - ft.Width / 2, c.Y - ft.Height / 2));
        }
    }

    private static void DrawWeightLabel(DrawingContext ctx, Point a, Point b, string text, IBrush color)
    {
        var mid = new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);
        var dir = new Vector(b.X - a.X, b.Y - a.Y);
        var len = dir.Length;
        if (len > 1e-9)
        {
            dir /= len;
            var perp = new Vector(-dir.Y, dir.X) * 10;
            mid += perp;
        }

        var ft = new FormattedText(
            text,
            System.Globalization.CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default),
            11,
            color);
        ctx.DrawText(ft, new Point(mid.X - ft.Width / 2, mid.Y - ft.Height / 2));
    }

    private static Point ToPixel(Point norm, Rect inner) =>
        new(inner.X + norm.X * inner.Width, inner.Y + norm.Y * inner.Height);

    private static Point ClipToCircle(Point from, Point toward, double r)
    {
        var dir = new Vector(toward.X - from.X, toward.Y - from.Y);
        var len = dir.Length;
        if (len < 1e-9)
            return from;
        dir /= len;
        return from + dir * r;
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
