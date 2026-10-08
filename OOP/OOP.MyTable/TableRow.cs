namespace OOP.MyTable;

public sealed class TableRow
{
    public TableRow(string name, long x1, long y1, long x2, long y2)
    {
        Name = name;
        X1 = x1;
        Y1 = y1;
        X2 = x2;
        Y2 = y2;
    }

    public string Name { get; }
    public long X1 { get; }
    public long Y1 { get; }
    public long X2 { get; }
    public long Y2 { get; }

    public override string ToString() => $"{Name}\t{X1}\t{Y1}\t{X2}\t{Y2}";
}
