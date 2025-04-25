public record BruteForceRequest
{
    public string  TargetIp     { get; init; } = "";
    public string  Username     { get; init; } = "";
    public List<string> Passwords { get; init; } = new();
    public int     AttemptCount { get; init; } = 0;      // optional
    public string? SourceIp     { get; init; } = null;   // optional
}