namespace Lab3Graph;

public sealed class VariantConfig
{
    public VariantConfig(int n1, int n2, int n3, int n4)
    {
        if (n1 is < 0 or > 9 || n2 is < 0 or > 9 || n3 is < 0 or > 9 || n4 is < 0 or > 9)
            throw new ArgumentOutOfRangeException("Each digit must be in 0..9.");

        N1 = n1;
        N2 = n2;
        N3 = n3;
        N4 = n4;

        Seed = N1 * 1000 + N2 * 100 + N3 * 10 + N4;
        VertexCount = 10 * N3;
        Layout = MapLayout(N4);
        K = 1.0 - N3 * 0.02 - N4 * 0.005 - 0.25;
    }

    public int N1 { get; }
    public int N2 { get; }
    public int N3 { get; }
    public int N4 { get; }

    public int Seed { get; }

    public int VertexCount { get; }

    public double K { get; }

    public VertexArrangement Layout { get; }

    private static VertexArrangement MapLayout(int n4) => n4 switch
    {
        0 or 1 => VertexArrangement.Circle,
        2 or 3 => VertexArrangement.Square,
        4 or 5 => VertexArrangement.Triangle,
        6 or 7 => VertexArrangement.CircleWithCenter,
        _ => VertexArrangement.SquareWithCenter
    };
}

public enum VertexArrangement
{
    Circle,
    Square,
    Triangle,
    CircleWithCenter,
    SquareWithCenter
}
