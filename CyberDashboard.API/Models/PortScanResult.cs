public class PortScanResult
{
    public string Ip { get; set; }
    public List<int> OpenPorts { get; set; } = new();
}