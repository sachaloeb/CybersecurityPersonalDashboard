public class PortScanResult
{
    public string Ip { get; set; } = string.Empty;
    public List<int> OpenPorts { get; set; } = new();
}