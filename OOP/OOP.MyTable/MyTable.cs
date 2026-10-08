namespace OOP.MyTable;


public sealed class MyTable
{
    private readonly List<TableRow> _rows = [];

    public event Action<IReadOnlyList<TableRow>>? RowsChanged;

    public event Action<int>? RowSelected;

    public IReadOnlyList<TableRow> Rows => _rows;

    public void Add(string name, long x1, long y1, long x2, long y2)
    {
        _rows.Add(new TableRow(name, x1, y1, x2, y2));
        RowsChanged?.Invoke(_rows);
    }

    public void Clear()
    {
        _rows.Clear();
        RowsChanged?.Invoke(_rows);
    }

    public void RemoveAt(int index)
    {
        if (index < 0 || index >= _rows.Count)
            return;

        _rows.RemoveAt(index);
        RowsChanged?.Invoke(_rows);
    }

    public void SelectRow(int index)
    {
        if (index < 0 || index >= _rows.Count)
            return;

        RowSelected?.Invoke(index);
    }
}
