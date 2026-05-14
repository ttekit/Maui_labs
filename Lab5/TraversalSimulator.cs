namespace Lab5Graph;

/// <summary>Состояние вершины: белая — не посещена, серая — в работе (очередь/стек), чёрная — завершена.</summary>
public enum VisitColor : byte
{
    White = 0,
    Gray = 1,
    Black = 2
}

public sealed class TraversalStep
{
    public required string Message { get; init; }
    public required byte[] VertexStates { get; init; }
    public required HashSet<(int From, int To)> TreeEdges { get; init; }
    public int? ActiveVertex { get; init; }
}

public static class TraversalSimulator
{
    public static List<TraversalStep> BuildBfs(int[,] g)
    {
        var steps = new List<TraversalStep>();
        var n = g.GetLength(0);
        var color = new byte[n];
        var tree = new HashSet<(int, int)>();

        void Push(string msg, int? active = null) =>
            steps.Add(new TraversalStep
            {
                Message = msg,
                VertexStates = (byte[])color.Clone(),
                TreeEdges = new HashSet<(int, int)>(tree),
                ActiveVertex = active
            });

        Push("Начало. Все вершины белые (не посещены). Древесные рёбра ещё нет. Используйте «Шаг вперёд» или →.");

        while (true)
        {
            var s = FindNextStart(g, color);
            if (s == null)
            {
                Push("Обход в ширину завершён: не осталось непосещённых вершин с исходящими дугами.");
                break;
            }

            Push($"Новый старт BFS: минимальная белая вершина с исходящей дугой — {V(s.Value)}.", s);
            var q = new Queue<int>();
            color[s.Value] = (byte)VisitColor.Gray;
            q.Enqueue(s.Value);
            Push($"В очередь помещена вершина {V(s.Value)} (серая — в очереди).", s);

            while (q.Count > 0)
            {
                var u = q.Dequeue();
                Push($"Из очереди извлечена вершина {V(u)}; обрабатываем исходящие рёбра по возрастанию номеров.", u);

                for (var v = 0; v < n; v++)
                {
                    if (g[u, v] != 1)
                        continue;

                    if (color[v] == (byte)VisitColor.White)
                    {
                        tree.Add((u, v));
                        color[v] = (byte)VisitColor.Gray;
                        q.Enqueue(v);
                        Push($"Дуга {V(u)}→{V(v)}: открыта новая вершина {V(v)} — древесное ребро (серая).", v);
                    }
                    else
                    {
                        var st = color[v] == (byte)VisitColor.Gray ? "серая (в очереди/обработке)" : "чёрная (завершена)";
                        Push($"Дуга {V(u)}→{V(v)}: не древесное ({V(v)} уже {st}).", u);
                    }
                }

                color[u] = (byte)VisitColor.Black;
                Push($"Вершина {V(u)} полностью обработана (чёрная).", u);
            }
        }

        return steps;
    }

    public static List<TraversalStep> BuildDfs(int[,] g)
    {
        var steps = new List<TraversalStep>();
        var n = g.GetLength(0);
        var color = new byte[n];
        var tree = new HashSet<(int, int)>();

        void Push(string msg, int? active = null) =>
            steps.Add(new TraversalStep
            {
                Message = msg,
                VertexStates = (byte[])color.Clone(),
                TreeEdges = new HashSet<(int, int)>(tree),
                ActiveVertex = active
            });

        Push("Начало. Все вершины белые. Используйте «Шаг вперёд» или →.");

        while (true)
        {
            var s = FindNextStart(g, color);
            if (s == null)
            {
                Push("Обход в глубину завершён.");
                break;
            }

            color[s.Value] = (byte)VisitColor.Gray;
            var stack = new List<Frame>();
            stack.Add(new Frame(s.Value, NeighborsAscending(g, s.Value), 0));
            Push($"Новый старт DFS: вершина {V(s.Value)} — серая, начало обхода со стека.", s);

            while (stack.Count > 0)
            {
                var li = stack.Count - 1;
                var top = stack[li];
                if (top.NextIdx < top.Nbrs.Count)
                {
                    var v = top.Nbrs[top.NextIdx];
                    top.NextIdx++;
                    stack[li] = top;

                    if (color[v] == (byte)VisitColor.White)
                    {
                        tree.Add((top.U, v));
                        color[v] = (byte)VisitColor.Gray;
                        stack.Add(new Frame(v, NeighborsAscending(g, v), 0));
                        Push($"Дуга {V(top.U)}→{V(v)}: древесное ребро, вход в {V(v)}.", v);
                    }
                    else
                    {
                        var st = color[v] == (byte)VisitColor.Gray ? "серая" : "чёрная";
                        Push($"Дуга {V(top.U)}→{V(v)}: не древесное ({V(v)} {st}).", top.U);
                    }
                }
                else
                {
                    var u = top.U;
                    stack.RemoveAt(li);
                    color[u] = (byte)VisitColor.Black;
                    Push($"Завершение вершины {V(u)} (чёрная), возврат.", u);
                }
            }
        }

        return steps;
    }

    private static int? FindNextStart(int[,] g, byte[] color)
    {
        var n = g.GetLength(0);
        for (var v = 0; v < n; v++)
        {
            if (color[v] != (byte)VisitColor.White)
                continue;
            if (!HasOutgoing(g, v, n))
                continue;
            return v;
        }

        return null;
    }

    private static bool HasOutgoing(int[,] g, int v, int n)
    {
        for (var j = 0; j < n; j++)
        {
            if (g[v, j] == 1)
                return true;
        }

        return false;
    }

    private static List<int> NeighborsAscending(int[,] g, int u)
    {
        var n = g.GetLength(0);
        var list = new List<int>();
        for (var v = 0; v < n; v++)
        {
            if (g[u, v] == 1)
                list.Add(v);
        }

        return list;
    }

    private static string V(int zeroBased) => (zeroBased + 1).ToString();

    private struct Frame
    {
        public int U;
        public List<int> Nbrs;
        public int NextIdx;

        public Frame(int u, List<int> nbrs, int nextIdx)
        {
            U = u;
            Nbrs = nbrs;
            NextIdx = nextIdx;
        }
    }
}
