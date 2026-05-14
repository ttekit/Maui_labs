namespace Lab6Graph;

public static class KruskalMst
{
    public static List<MstStep> BuildSteps(int[,] w, AdjacencyListGraph? graph = null)
    {
        _ = graph;
        var n = w.GetLength(0);
        var edges = new List<(int u, int v, int wt)>();
        for (var i = 0; i < n; i++)
        for (var j = i + 1; j < n; j++)
        {
            var wt = w[i, j];
            if (wt > 0)
                edges.Add((i, j, wt));
        }

        edges.Sort(static (a, b) =>
        {
            var c = a.wt.CompareTo(b.wt);
            if (c != 0)
                return c;
            c = a.u.CompareTo(b.u);
            return c != 0 ? c : a.v.CompareTo(b.v);
        });

        var parent = new int[n];
        var rank = new int[n];
        for (var i = 0; i < n; i++)
            parent[i] = i;

        var mst = new HashSet<(int, int)>();
        var steps = new List<MstStep>();

        void Push(string msg, (int U, int V)? cand = null, int? cwt = null) =>
            steps.Add(new MstStep
            {
                Message = msg,
                MstEdges = new HashSet<(int, int)>(mst),
                CandidateEdge = cand,
                CandidateWeight = cwt
            });

        Push("Краскал: отсортированы все рёбра по неубыванию веса. Нажмите «Шаг вперёд».");

        foreach (var (u, v, wt) in edges)
        {
            var e = (u, v);
            Push($"Рассматриваем ребро {V(u)}—{V(v)}, вес {wt}.", e, wt);

            var ru = Find(parent, u);
            var rv = Find(parent, v);
            if (ru == rv)
            {
                Push($"Ребро {V(u)}—{V(v)} образует цикл — не включаем.");
                continue;
            }

            Union(parent, rank, ru, rv);
            mst.Add(e);
            Push($"Ребро {V(u)}—{V(v)} (вес {wt}) добавлено в минимальный остовной лес.");
        }

        Push($"Краскал завершён. Рёбер в остове: {mst.Count}.");
        return steps;
    }

    private static int Find(int[] parent, int x)
    {
        while (parent[x] != x)
            x = parent[x];
        return x;
    }

    private static void Union(int[] parent, int[] rank, int a, int b)
    {
        if (rank[a] < rank[b])
            (a, b) = (b, a);
        parent[b] = a;
        if (rank[a] == rank[b])
            rank[a]++;
    }

    private static string V(int i) => (i + 1).ToString();
}
