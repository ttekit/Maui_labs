using System.Text;

namespace Lab2QueueReverse;

internal static class QueueConsole
{
    internal static void Run()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.Write("Enter the number of elements n (n > 0): ");
        if (!int.TryParse(Console.ReadLine(), out var n) || n <= 0)
        {
            Console.WriteLine("Invalid input: expected a positive integer.");
            return;
        }

        var queue = CreateQueueFromInput(n);
        Console.Write("Original queue (front → rear): ");
        PrintQueue(queue);

        var reversed = new CharQueue();
        ReverseQueueToQueue(queue, reversed);

        Console.Write("Reversed queue (front → rear): ");
        PrintQueue(reversed);

        FreeQueue(queue);
        FreeQueue(reversed);
    }

    private static CharQueue CreateQueueFromInput(int n)
    {
        var q = new CharQueue();
        Console.WriteLine($"Enter {n} characters (single line or separated by spaces):");
        var line = Console.ReadLine() ?? string.Empty;
        var chars = CollectChars(line, n);
        for (var i = 0; i < n; i++)
            q.Enqueue(chars[i]);
        return q;
    }

    private static char[] CollectChars(string line, int n)
    {
        var list = new List<char>(n);
        foreach (var ch in line)
        {
            if (!char.IsWhiteSpace(ch))
                list.Add(ch);
            if (list.Count == n)
                break;
        }

        while (list.Count < n)
        {
            Console.Write($"Need {n - list.Count} more character(s): ");
            var extra = Console.ReadLine() ?? string.Empty;
            foreach (var ch in extra)
            {
                if (!char.IsWhiteSpace(ch))
                    list.Add(ch);
                if (list.Count == n)
                    break;
            }
        }

        return list.ToArray();
    }

    private static void ReverseQueueToQueue(CharQueue source, CharQueue destination)
    {
        if (source.IsEmpty)
            return;

        var x = source.Dequeue();
        ReverseQueueToQueue(source, destination);
        destination.Enqueue(x);
    }

    private static void PrintQueue(CharQueue queue)
    {
        if (queue.IsEmpty)
        {
            Console.WriteLine("(empty)");
            return;
        }

        var temp = new CharQueue();
        while (!queue.IsEmpty)
        {
            var ch = queue.Dequeue();
            Console.Write(ch);
            Console.Write(' ');
            temp.Enqueue(ch);
        }

        while (!temp.IsEmpty)
            queue.Enqueue(temp.Dequeue());
        Console.WriteLine();
    }

    private static void FreeQueue(CharQueue queue) => queue.Clear();
}
