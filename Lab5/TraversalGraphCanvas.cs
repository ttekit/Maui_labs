using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Lab5Graph;

/// <summary>Ориентированный граф: древесные рёбра выделяются цветом, вершины — по фазам обхода.</summary>
public sealed class TraversalGraphCanvas : Control
{
    private int[,]? _matrix;
    private Point[]? _positions;
    private byte[]? _vertexStates;
    private HashSet<(int From, int To)>? _treeEdges;
    private int? _activeVertex;

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

    public byte[]? VertexStates
    {
        get => _vertexStates;
        set
        {
            _vertexStates = value;
            InvalidateVisual();
        }
    }

    public HashSet<(int From, int To)>? TreeEdges
    {
        get => _treeEdges;
        set
        {
            _treeEdges = value;
            InvalidateVisual();
        }
    }

    public int? ActiveVertex
    {
        get => _activeVertex;
        set
        {
            _activeVertex = value;
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

        var states = _vertexStates;
        if (states == null || states.Length != n)
            states = new byte[n];

        var tree = _treeEdges ?? new HashSet<(int, int)>();

        const double margin = 28;
        var inner = rect.Deflate(margin);
        var rVert = Math.Min(inner.Width, inner.Height) * 0.028;
        if (rVert < 2) rVert = 2;

        var pixels = new Point[n];
        for (var i = 0; i < n; i++)
            pixels[i] = ToPixel(pos[i], inner);

        var penBack = new Pen(new SolidColorBrush(Color.FromRgb(200, 200, 200)), 1.05);
        var penTree = new Pen(new SolidColorBrush(Color.FromRgb(0, 140, 60)), 2.6);
        var loopBack = new Pen(new SolidColorBrush(Color.FromRgb(190, 190, 190)), 1.0);
        var loopTree = new Pen(new SolidColorBrush(Color.FromRgb(0, 120, 55)), 2.2);
        var head = Math.Max(6, rVert * 1.1);

        for (var pass = 0; pass < 2; pass++)
        {
            for (var i = 0; i < n; i++)
            for (var j = 0; j < n; j++)
            {
                if (m[i, j] != 1)
                    continue;

                var isTree = tree.Contains((i, j));
                if (pass == 0 && isTree)
                    continue;
                if (pass == 1 && !isTree)
                    continue;

                var pen = isTree ? penTree : penBack;
                var loopPen = isTree ? loopTree : loopBack;

                if (i == j)
                    DrawSelfLoop(context, pixels[i], rVert, loopPen);
                else
                {
                    var bi = m[j, i] == 1;
                    var shift = bi ? PerpOffset(pixels[i], pixels[j], rVert * 0.9) : default;
                    var sign = bi ? (i < j ? 1.0 : -1.0) : 0.0;
                    var off = shift * sign;
                    DrawDirectedEdge(context, pen, pixels[i], pixels[j], rVert, head, off,
                        isTree ? new SolidColorBrush(Color.FromRgb(0, 100, 45)) : Brushes.Gray);
                }
            }
        }

        for (var i = 0; i < n; i++)
        {
            var c = pixels[i];
            var fill = StateFill(states[i]);
            var stroke = StateStroke(states[i]);
            var vr = rVert;
            var vrGeom = new EllipseGeometry(new Rect(c.X - vr, c.Y - vr, 2 * vr, 2 * vr));
            context.DrawGeometry(fill, new Pen(new SolidColorBrush(stroke), 1.15), vrGeom);

            if (_activeVertex == i)
            {
                var ring = new EllipseGeometry(new Rect(c.X - vr - 4, c.Y - vr - 4, 2 * (vr + 4), 2 * (vr + 4)));
                context.DrawGeometry(null, new Pen(Brushes.Crimson, 2.4), ring);
            }

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

    private static IBrush StateFill(byte s) => s switch
    {
        (byte)VisitColor.White => new SolidColorBrush(Color.FromRgb(220, 220, 225)),
        (byte)VisitColor.Gray => new SolidColorBrush(Color.FromRgb(245, 160, 50)),
        (byte)VisitColor.Black => new SolidColorBrush(Color.FromRgb(55, 105, 175)),
        _ => Brushes.LightGray
    };

    private static Color StateStroke(byte s) => s switch
    {
        (byte)VisitColor.White => Color.FromRgb(130, 130, 140),
        (byte)VisitColor.Gray => Color.FromRgb(160, 90, 20),
        (byte)VisitColor.Black => Color.FromRgb(25, 55, 110),
        _ => Colors.Gray
    };

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

    private static void DrawDirectedEdge(
        DrawingContext ctx,
        Pen pen,
        Point from,
        Point to,
        double rVert,
        double headLen,
        Vector parallelOffset,
        IBrush headFill)
    {
        var f = from + parallelOffset;
        var t = to + parallelOffset;
        var dir = new Vector(t.X - f.X, t.Y - f.Y);
        var len = dir.Length;
        if (len < 1e-9)
            return;
        dir /= len;
        f = f + dir * rVert;
        t = t - dir * (rVert + headLen * 0.35);

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

        ctx.DrawGeometry(headFill, null, geo);
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
