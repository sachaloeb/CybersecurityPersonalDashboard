public interface IPortScannerService
{
    Task<DetailedPortScanResult> ScanPortsDetailedAsync(PortScanRequest request);
}