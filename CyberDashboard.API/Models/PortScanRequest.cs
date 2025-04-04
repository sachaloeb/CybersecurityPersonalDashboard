public class PortScanRequest
{
    public string Ip { get; set; } = string.Empty;
    public int StartPort { get; set; }
    public int EndPort { get; set; }
}