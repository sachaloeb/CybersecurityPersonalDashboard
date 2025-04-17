using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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

        // Change ScanPortsAsync to ScanPortsDetailedAsync
        var result = await _scannerService.ScanPortsDetailedAsync(request);
        return Ok(result);
    }
    
    [HttpPost("scan-detailed")]
    public async Task<IActionResult> ScanPortsDetailed([FromBody] PortScanRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Ip) || request.StartPort < 1 || 
            request.EndPort > 65535 || request.StartPort > request.EndPort)
        {
            return BadRequest("Invalid IP address or port range.");
        }

        var result = await _scannerService.ScanPortsDetailedAsync(request);
        return Ok(result);
    }
    
    [HttpGet("history")]
    public async Task<IActionResult> History([FromServices] MongoLogService mongo,
        [FromQuery] int take = 20)
    {
        var latest = await mongo.PortScanLogs
            .Find(_ => true)
            .SortByDescending(l => l.StartedAt)
            .Limit(take)
            .Project(l => new
            {
                l.Host, l.StartPort, l.EndPort,
                l.StartedAt, l.EndedAt,
                l.Duration, l.Status
            })
            .ToListAsync();
        return Ok(latest);
    }
}