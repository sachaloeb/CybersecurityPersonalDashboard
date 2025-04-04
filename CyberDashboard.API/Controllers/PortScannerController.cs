using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PortScannerController : ControllerBase
{
    private readonly IPortScannerService _scannerService;

    public PortScannerController(IPortScannerService scannerService)
    {
        _scannerService = scannerService;
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanPorts([FromBody] PortScanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Ip) || request.StartPort < 1 || 
            request.EndPort > 65535 || request.StartPort > request.EndPort)
        {
            return BadRequest("Invalid IP address or port range.");
        }

        var result = await _scannerService.ScanPortsAsync(request);
        return Ok(result);
    }
}