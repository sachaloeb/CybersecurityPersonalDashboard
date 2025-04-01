public class NetworkConnection
{
    public required string Local { get; set; }
    public int LocalPort { get; set; }
    public required string Remote { get; set; }
    public int RemotePort { get; set; }
    public string? Protocol { get; set; } // Made nullable
    public required string State { get; set; }
}