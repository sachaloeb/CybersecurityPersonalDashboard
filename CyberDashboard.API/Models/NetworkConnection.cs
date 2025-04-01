public class NetworkConnection
{
    public string Local { get; set; } = string.Empty; // or make it nullable: public string? Local { get; set; }
    public string Remote { get; set; } = string.Empty; // or make it nullable: public string? Remote { get; set; }
    public string State { get; set; } = string.Empty; // or make it nullable: public string? State { get; set; }
}