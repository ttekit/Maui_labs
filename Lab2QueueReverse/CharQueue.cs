namespace Lab2QueueReverse;

internal sealed class CharQueue
{
    private CharNode? _front;
    private CharNode? _rear;

    public bool IsEmpty => _front is null;

    public void Enqueue(char key)
    {
        var node = new CharNode(key);
        if (_rear is null)
        {
            _front = _rear = node;
            return;
        }

        _rear.Next = node;
        _rear = node;
    }

    public char Dequeue()
    {
        if (_front is null)
            throw new InvalidOperationException("Queue is empty.");

        var key = _front.Key;
        _front = _front.Next;
        if (_front is null)
            _rear = null;
        return key;
    }

    public void Clear()
    {
        while (!IsEmpty)
            Dequeue();
    }
}
