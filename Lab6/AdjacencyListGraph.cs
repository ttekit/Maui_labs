namespace Lab6Graph;

public sealed class AdjacencyListGraph
{
    private readonly List<List<(int Neighbor, int Weight)>> _adj;

    public AdjacencyListGraph(int vertexCount)
    {
        if (vertexCount < 0)
            throw new ArgumentOutOfRangeException(nameof(vertexCount));
        _adj = new List<List<(int, int)>>(vertexCount);
        for (var i = 0; i < vertexCount; i++)
            _adj.Add(new List<(int, int)>());
    }

    public int VertexCount => _adj.Count;

    public IReadOnlyList<(int Neighbor, int Weight)> Neighbors(int v) => _adj[v];

    public void AddVertex()
    {
        _adj.Add(new List<(int, int)>());
    }

    public void AddUndirectedEdge(int u, int v, int w)
    {
        if (u == v || w <= 0)
            return;
        if ((uint)u >= (uint)_adj.Count || (uint)v >= (uint)_adj.Count)
            throw new ArgumentOutOfRangeException();
        AddHalf(u, v, w);
        AddHalf(v, u, w);
    }

    private void AddHalf(int from, int to, int w)
    {
        var list = _adj[from];
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].Neighbor == to)
            {
                if (list[i].Weight != w)
                    list[i] = (to, w);
                return;
            }
        }

        list.Add((to, w));
    }

    public bool RemoveUndirectedEdge(int u, int v)
    {
        if ((uint)u >= (uint)_adj.Count || (uint)v >= (uint)_adj.Count)
            return false;
        var a = RemoveHalf(u, v);
        var b = RemoveHalf(v, u);
        return a || b;
    }

    private bool RemoveHalf(int from, int to)
    {
        var list = _adj[from];
        for (var i = 0; i < list.Count; i++)
        {
            if (list[i].Neighbor != to)
                continue;
            list.RemoveAt(i);
            return true;
        }

        return false;
    }

    public static AdjacencyListGraph FromWeightMatrix(int[,] w)
    {
        var n = w.GetLength(0);
        var g = new AdjacencyListGraph(n);
        for (var i = 0; i < n; i++)
        for (var j = i + 1; j < n; j++)
        {
            var wt = w[i, j];
            if (wt > 0)
                g.AddUndirectedEdge(i, j, wt);
        }

        return g;
    }
}
