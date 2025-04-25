public record XssRequest
{
    public string TargetUrl { get; init; } = "";
    public string Payload   { get; init; } = "";
}