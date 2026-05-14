namespace Lab6Graph;

public static class PrimMst
{
    public static List<MstStep> BuildSteps(int[,] w, AdjacencyListGraph? graph = null)
    {
        _ = graph;
        var n = w.GetLength(0);
        var assigned = new bool[n];
        var mst = new HashSet<(int, int)>();
        var steps = new List<MstStep>();
        var inTree = new HashSet<int>();

        void Push(string msg, (int U, int V)? cand = null, int? cwt = null) =>
            steps.Add(new MstStep
            {
                Message = msg,
                MstEdges = new HashSet<(int, int)>(mst),
                CandidateEdge = cand,
                CandidateWeight = cwt
            });

        Push("Прим: построение минимального остовного леса. Нажмите «Шаг вперёд».");

        while (true)
        {
            var s = -1;
            for (var i = 0; i < n; i++)
            {
                if (!assigned[i])
                {
                    s = i;
                    break;
                }
            }

            if (s < 0)
                break;

            var hasUnassignedNeighbor = false;
            for (var j = 0; j < n; j++)
            {
                if (assigned[j] || j == s)
                    continue;
                if (w[s, j] > 0)
                {
                    hasUnassignedNeighbor = true;
                    break;
                }
            }

            if (!hasUnassignedNeighbor)
            {
                assigned[s] = true;
                Push($"Вершина {V(s)} — компонента из одной вершины (нет рёбер к ещё не охваченным).");
                continue;
            }

            inTree.Clear();
            inTree.Add(s);
            assigned[s] = true;
            Push($"Новая компонента остова: корень — вершина {V(s)}.");

            var heap = new List<(int Wt, int V, int P)>();
            Relax(w, assigned, heap, s);

            while (heap.Count > 0)
            {
                var bi = 0;
                for (var k = 1; k < heap.Count; k++)
                {
                    if (IsBetter(heap[k], heap[bi]))
                        bi = k;
                }

                var (wt0, v, p) = heap[bi];
                heap.RemoveAt(bi);

                Push($"Кандидат: ребро {V(p)}—{V(v)}, вес {wt0}.", Norm(p, v), wt0);

                if (inTree.Contains(v))
                {
                    Push("Вершина уже в текущем дереве — пропуск (устаревшая запись).");
                    continue;
                }

                if (assigned[v])
                {
                    Push("Вершина уже в остовном лесе — пропуск.");
                    continue;
                }

                var edge = Norm(p, v);
                mst.Add(edge);
                assigned[v] = true;
                inTree.Add(v);
                Push($"Ребро {V(edge.U)}—{V(edge.V)} (вес {wt0}) включено в остов.");
                Relax(w, assigned, heap, v);
            }
        }

        Push($"Прим завершён. Рёбер в остове: {mst.Count}.");
        return steps;
    }

    private static void Relax(int[,] w, bool[] assigned, List<(int Wt, int V, int P)> heap, int x)
    {
        var n = w.GetLength(0);
        for (var y = 0; y < n; y++)
        {
            if (y == x)
                continue;
            var wt = w[x, y];
            if (wt <= 0)
                continue;
            if (assigned[y])
                continue;
            heap.Add((wt, y, x));
        }
    }

    private static bool IsBetter((int Wt, int V, int P) a, (int Wt, int V, int P) b)
    {
        if (a.Wt != b.Wt)
            return a.Wt < b.Wt;
        if (a.V != b.V)
            return a.V < b.V;
        return a.P < b.P;
    }

    private static (int U, int V) Norm(int a, int b) => a < b ? (a, b) : (b, a);

    private static string V(int i) => (i + 1).ToString();
}
