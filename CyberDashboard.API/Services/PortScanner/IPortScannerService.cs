public interface IPortScannerService
{
    Task<PortScanResult> ScanPortsAsync(PortScanRequest request);
}