namespace Lab6Graph;

public static class GraphTraversalLists
{
    public static List<int> BreadthFirstOrder(AdjacencyListGraph g, int start)
    {
        if ((uint)start >= (uint)g.VertexCount)
            throw new ArgumentOutOfRangeException(nameof(start));
        var order = new List<int>();
        var seen = new bool[g.VertexCount];
        var q = new Queue<int>();
        seen[start] = true;
        q.Enqueue(start);
        while (q.Count > 0)
        {
            var v = q.Dequeue();
            order.Add(v);
            foreach (var (nbr, _) in g.Neighbors(v).OrderBy(t => t.Neighbor))
            {
                if (seen[nbr])
                    continue;
                seen[nbr] = true;
                q.Enqueue(nbr);
            }
        }

        return order;
    }

    public static List<int> DepthFirstOrder(AdjacencyListGraph g, int start)
    {
        if ((uint)start >= (uint)g.VertexCount)
            throw new ArgumentOutOfRangeException(nameof(start));
        var order = new List<int>();
        var seen = new bool[g.VertexCount];
        var stack = new Stack<int>();
        stack.Push(start);
        while (stack.Count > 0)
        {
            var v = stack.Pop();
            if (seen[v])
                continue;
            seen[v] = true;
            order.Add(v);
            var ordered = g.Neighbors(v).OrderBy(t => t.Neighbor).ToList();
            for (var i = ordered.Count - 1; i >= 0; i--)
            {
                var nbr = ordered[i].Neighbor;
                if (!seen[nbr])
                    stack.Push(nbr);
            }
        }

        return order;
    }
}
