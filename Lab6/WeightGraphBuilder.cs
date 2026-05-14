namespace Lab6Graph;


public static class WeightGraphBuilder
{
    public sealed class Result
    {
        public required int[,] Adir { get; init; }
        public required int[,] Aundir { get; init; }
        public required double[,] B { get; init; }
        public required int[,] C { get; init; }
        public required int[,] D { get; init; }
        public required int[,] H { get; init; }
        public required int[,] W { get; init; }
    }

    public static Result Build(VariantConfig cfg)
    {
        var n = cfg.VertexCount;
        var rnd = new Random(cfg.Seed);

        var adir = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            var t = rnd.NextDouble() * 2.0;
            adir[i, j] = t * cfg.K < 1.0 ? 0 : 1;
        }

        var aund = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            aund[i, j] = adir[i, j] == 1 || adir[j, i] == 1 ? 1 : 0;

        var b = new double[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            b[i, j] = rnd.NextDouble() * 2.0;

        var c = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            c[i, j] = (int)Math.Ceiling(b[i, j] * 100.0 * aund[i, j]);

        var d = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            d[i, j] = c[i, j] > 0 ? 1 : 0;

        var h = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            h[i, j] = d[i, j] == d[j, i] ? 1 : 0;

        var w = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = i; j < n; j++)
        {
            var tr = i <= j ? 1 : 0;
            var val = d[i, j] * h[i, j] * tr * c[i, j];
            w[i, j] = val;
            w[j, i] = val;
        }

        return new Result
        {
            Adir = adir,
            Aundir = aund,
            B = b,
            C = c,
            D = d,
            H = h,
            W = w
        };
    }

    public static string FormatIntMatrix(int[,] m)
    {
        var n0 = m.GetLength(0);
        var n1 = m.GetLength(1);
        var lines = new string[n0];
        for (var i = 0; i < n0; i++)
        {
            var parts = new string[n1];
            for (var j = 0; j < n1; j++)
                parts[j] = m[i, j].ToString();
            lines[i] = string.Join(' ', parts);
        }

        return string.Join(Environment.NewLine, lines);
    }

    public static string FormatDoubleMatrix(double[,] m, string format = "F4")
    {
        var n0 = m.GetLength(0);
        var n1 = m.GetLength(1);
        var lines = new string[n0];
        for (var i = 0; i < n0; i++)
        {
            var parts = new string[n1];
            for (var j = 0; j < n1; j++)
                parts[j] = m[i, j].ToString(format);
            lines[i] = string.Join(' ', parts);
        }

        return string.Join(Environment.NewLine, lines);
    }
}
