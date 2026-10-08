namespace OOP.Shapes;

using OOP.Shapes.Shapes;


public sealed class ShapeStorage
{
    private readonly EditorSettings _settings;
    private readonly Shape?[] _shapes;
    private int _count;
    private bool _capacityWarningSent;

    public ShapeStorage(EditorSettings settings)
    {
        _settings = settings;
        _shapes = new Shape?[settings.ArrayCapacity];
        _count = 0;
    }

    public EditorSettings Settings => _settings;

    public int Count => _count;

    public int Capacity => _shapes.Length;

    public bool IsFull => _count >= _shapes.Length;

    public IReadOnlyList<Shape?> Shapes => _shapes;

    public event Action? CapacityReached;

    public bool TryAdd(Shape shape)
    {
        if (_count >= _shapes.Length)
        {
            if (!_capacityWarningSent)
            {
                _capacityWarningSent = true;
                CapacityReached?.Invoke();
            }

            return false;
        }

        _shapes[_count] = shape;
        _count++;
        return true;
    }

    public void Clear()
    {
        Array.Clear(_shapes, 0, _shapes.Length);
        _count = 0;
        _capacityWarningSent = false;
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _count)
            return;

        for (int i = index; i < _count - 1; i++)
            _shapes[i] = _shapes[i + 1];

        _shapes[_count - 1] = null;
        _count--;
    }

    public IEnumerable<Shape> EnumerateShapes()
    {
        for (int i = 0; i < _count; i++)
        {
            if (_shapes[i] is Shape shape)
                yield return shape;
        }
    }
}
