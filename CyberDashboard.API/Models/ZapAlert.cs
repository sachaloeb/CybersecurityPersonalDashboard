public class ZapAlert
{
    public required string Alert { get; set; }
    public required string Risk { get; set; }
    public required string Url { get; set; }
    public string? Description { get; set; }
    public string? Solution { get; set; }
    public string? Reference { get; set; }
    public Dictionary<string, string>? Tags { get; set; }
}