using Avalonia;

namespace Lab3Graph;

public static class VertexLayout
{
    public static Point[] Compute(VertexArrangement arrangement, int n)
    {
        if (n <= 0)
            return Array.Empty<Point>();

        return arrangement switch
        {
            VertexArrangement.Circle => Circle(n, 0.5, 0.5, 0.38),
            VertexArrangement.Square => SquarePerimeter(n, 0.5, 0.5, 0.40, 0.32),
            VertexArrangement.Triangle => TrianglePerimeter(n, 0.5, 0.5, 0.42),
            VertexArrangement.CircleWithCenter => CircleWithCenter(n, 0.5, 0.5, 0.38),
            VertexArrangement.SquareWithCenter => SquareWithCenter(n, 0.5, 0.5, 0.40, 0.32),
            _ => Circle(n, 0.5, 0.5, 0.38)
        };
    }

    private static Point[] Circle(int n, double cx, double cy, double r)
    {
        var pts = new Point[n];
        for (var i = 0; i < n; i++)
        {
            var a = -Math.PI / 2 + 2 * Math.PI * i / n;
            pts[i] = new Point(cx + r * Math.Cos(a), cy + r * Math.Sin(a));
        }

        return pts;
    }

    private static Point[] CircleWithCenter(int n, double cx, double cy, double r)
    {
        if (n == 1)
            return [new Point(cx, cy)];

        var pts = new Point[n];
        pts[0] = new Point(cx, cy);
        var m = n - 1;
        for (var k = 0; k < m; k++)
        {
            var a = -Math.PI / 2 + 2 * Math.PI * k / m;
            pts[k + 1] = new Point(cx + r * Math.Cos(a), cy + r * Math.Sin(a));
        }

        return pts;
    }

    private static Point[] SquarePerimeter(int n, double cx, double cy, double halfW, double halfH)
    {
        var pts = new Point[n];
        if (n == 0)
            return pts;

        var top = halfW * 2;
        var right = halfH * 2;
        var bottom = top;
        var left = right;
        var total = top + right + bottom + left;

        for (var i = 0; i < n; i++)
        {
            var dist = total * (i + 0.5) / n;
            pts[i] = PointOnRectanglePerimeter(cx, cy, halfW, halfH, dist, top, right, bottom, left);
        }

        return pts;
    }

    private static Point[] SquareWithCenter(int n, double cx, double cy, double halfW, double halfH)
    {
        if (n == 1)
            return [new Point(cx, cy)];

        var pts = new Point[n];
        pts[0] = new Point(cx, cy);
        var m = n - 1;
        var top = halfW * 2;
        var right = halfH * 2;
        var bottom = top;
        var left = right;
        var total = top + right + bottom + left;
        for (var k = 0; k < m; k++)
        {
            var dist = total * (k + 0.5) / m;
            pts[k + 1] = PointOnRectanglePerimeter(cx, cy, halfW, halfH, dist, top, right, bottom, left);
        }

        return pts;
    }

    private static Point PointOnRectanglePerimeter(
        double cx, double cy, double halfW, double halfH, double dist,
        double top, double right, double bottom, double left)
    {
        double x0 = cx - halfW, y0 = cy - halfH;
        double x1 = cx + halfW, y1 = cy + halfH;
        double t = dist;
        if (t <= top)
            return new Point(x0 + t, y0);
        t -= top;
        if (t <= right)
            return new Point(x1, y0 + t);
        t -= right;
        if (t <= bottom)
            return new Point(x1 - t, y1);
        t -= bottom;
        return new Point(x0, y1 - t);
    }

    private static Point[] TrianglePerimeter(int n, double cx, double cy, double r)
    {
        var pts = new Point[n];
        var v0 = new Point(cx, cy - r);
        var v1 = new Point(cx - r * Math.Sin(Math.PI / 3), cy + r * Math.Cos(Math.PI / 3));
        var v2 = new Point(cx + r * Math.Sin(Math.PI / 3), cy + r * Math.Cos(Math.PI / 3));
        var e01 = Dist(v0, v1);
        var e12 = Dist(v1, v2);
        var e20 = Dist(v2, v0);
        var total = e01 + e12 + e20;

        for (var i = 0; i < n; i++)
        {
            var d = total * (i + 0.5) / n;
            if (d <= e01)
            {
                var t = d / e01;
                pts[i] = Lerp(v0, v1, t);
            }
            else if (d <= e01 + e12)
            {
                var t = (d - e01) / e12;
                pts[i] = Lerp(v1, v2, t);
            }
            else
            {
                var t = (d - e01 - e12) / e20;
                pts[i] = Lerp(v2, v0, t);
            }
        }

        return pts;
    }

    private static double Dist(Point a, Point b)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static Point Lerp(Point a, Point b, double t) =>
        new Point(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
}
