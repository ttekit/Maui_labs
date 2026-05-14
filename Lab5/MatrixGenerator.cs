namespace Lab5Graph;

public static class MatrixGenerator
{
    public static int[,] BuildDirected(VariantConfig cfg)
    {
        var n = cfg.VertexCount;
        var rnd = new Random(cfg.Seed);
        var a = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            var t = rnd.NextDouble() * 2.0;
            var v = t * cfg.K;
            a[i, j] = v < 1.0 ? 0 : 1;
        }

        return a;
    }

    public static string FormatMatrix(int[,] m)
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
}
