namespace Lab6Graph;

public sealed class MstStep
{
    public required string Message { get; init; }
    public required HashSet<(int U, int V)> MstEdges { get; init; }
    public (int U, int V)? CandidateEdge { get; init; }
    public int? CandidateWeight { get; init; }
}
