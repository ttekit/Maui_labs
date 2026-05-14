namespace Lab4Graph;

public static class GraphAnalysis
{
    public static int[] OutDegrees(int[,] a)
    {
        var n = a.GetLength(0);
        var d = new int[n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            d[i] += a[i, j];
        return d;
    }

    public static int[] InDegrees(int[,] a)
    {
        var n = a.GetLength(0);
        var d = new int[n];
        for (var j = 0; j < n; j++)
        for (var i = 0; i < n; i++)
            d[j] += a[i, j];
        return d;
    }

    public static int[] UndirectedDegrees(int[,] u)
    {
        var n = u.GetLength(0);
        var d = new int[n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            d[i] += u[i, j];
        return d;
    }

    public static bool TryRegularUndirected(int[] degrees, out int k)
    {
        k = 0;
        if (degrees.Length == 0)
            return false;
        k = degrees[0];
        for (var i = 1; i < degrees.Length; i++)
        {
            if (degrees[i] != k)
                return false;
        }

        return true;
    }

    public static bool TryRegularDirected(int[] inDeg, int[] outDeg, out int kIn, out int kOut)
    {
        kIn = kOut = 0;
        if (inDeg.Length == 0)
            return false;
        kIn = inDeg[0];
        kOut = outDeg[0];
        for (var i = 1; i < inDeg.Length; i++)
        {
            if (inDeg[i] != kIn || outDeg[i] != kOut)
                return false;
        }

        return true;
    }

    public static int[,] MatrixPowerCount(int[,] a, int power)
    {
        var n = a.GetLength(0);
        var cur = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            cur[i, j] = a[i, j];
        for (var p = 1; p < power; p++)
            cur = MultiplyCount(cur, a);
        return cur;
    }

    private static int[,] MultiplyCount(int[,] x, int[,] y)
    {
        var n = x.GetLength(0);
        var z = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            var s = 0;
            for (var k = 0; k < n; k++)
                s += x[i, k] * y[k, j];
            z[i, j] = s;
        }

        return z;
    }

    public static List<string> PathsLength2(int[,] a)
    {
        var n = a.GetLength(0);
        var list = new List<string>();
        for (var i = 0; i < n; i++)
        for (var k = 0; k < n; k++)
        {
            if (a[i, k] == 0)
                continue;
            for (var j = 0; j < n; j++)
            {
                if (a[k, j] == 0)
                    continue;
                list.Add($"{i + 1} - {k + 1} - {j + 1}");
            }
        }

        return list;
    }

    public static List<string> PathsLength3(int[,] a)
    {
        var n = a.GetLength(0);
        var list = new List<string>();
        for (var i = 0; i < n; i++)
        for (var k = 0; k < n; k++)
        {
            if (a[i, k] == 0)
                continue;
            for (var l = 0; l < n; l++)
            {
                if (a[k, l] == 0)
                    continue;
                for (var j = 0; j < n; j++)
                {
                    if (a[l, j] == 0)
                        continue;
                    list.Add($"{i + 1} - {k + 1} - {l + 1} - {j + 1}");
                }
            }
        }

        return list;
    }

    public static int[,] TransitiveClosure(int[,] a)
    {
        var n = a.GetLength(0);
        var r = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            r[i, j] = i == j || a[i, j] == 1 ? 1 : 0;

        for (var k = 0; k < n; k++)
        for (var i = 0; i < n; i++)
        {
            if (r[i, k] == 0)
                continue;
            for (var j = 0; j < n; j++)
            {
                if (r[k, j] == 1)
                    r[i, j] = 1;
            }
        }

        return r;
    }

    public static int[,] StrongConnectivityMatrix(int[,] reach)
    {
        var n = reach.GetLength(0);
        var s = new int[n, n];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
            s[i, j] = reach[i, j] == 1 && reach[j, i] == 1 ? 1 : 0;
        return s;
    }

    public static List<List<int>> StronglyConnectedComponents(int[,] strong)
    {
        var n = strong.GetLength(0);
        var used = new bool[n];
        var comps = new List<List<int>>();
        for (var i = 0; i < n; i++)
        {
            if (used[i])
                continue;
            var comp = new List<int>();
            for (var j = 0; j < n; j++)
            {
                if (strong[i, j] == 1)
                {
                    comp.Add(j);
                    used[j] = true;
                }
            }

            comps.Add(comp);
        }

        return comps;
    }

    public static (int[,] adj, int[] vertexToComp) Condensation(int[,] a, List<List<int>> components)
    {
        var n = a.GetLength(0);
        var c = components.Count;
        var map = new int[n];
        for (var ci = 0; ci < c; ci++)
        foreach (var v in components[ci])
            map[v] = ci;

        var adj = new int[c, c];
        for (var i = 0; i < n; i++)
        for (var j = 0; j < n; j++)
        {
            if (a[i, j] == 0)
                continue;
            var u = map[i];
            var v = map[j];
            if (u != v)
                adj[u, v] = 1;
        }

        return (adj, map);
    }
}
