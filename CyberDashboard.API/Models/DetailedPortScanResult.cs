using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PortState
{
    Open,
    Closed,
    Filtered
}

public class PortStatusInfo
{
    public int Port { get; set; }
    public PortState State { get; set; }
}

public class DetailedPortScanResult
{
    public string Ip { get; set; }
    public bool IsHostUp { get; set; }
    public TimeSpan ScanDuration { get; set; }
    public List<PortStatusInfo> Ports { get; set; } = new();
    public ScanStatus Status { get; set; } = ScanStatus.Complete;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}