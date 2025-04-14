// Controllers/ThreatSimulationController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ThreatSimulationController : ControllerBase
{
    private readonly ThreatSimulationService _threatService;
    private readonly ThreatLogService _logService;

    public ThreatSimulationController(ThreatSimulationService threatService, ThreatLogService logService)
    {
        _threatService = threatService;
        _logService = logService;
    }

    // 1) BRUTE FORCE
    [HttpPost("brute")]
    public async Task<IActionResult> SimulateBrute(
        [FromBody] BruteForceRequest req)
    {
        var logs = await _threatService.SimulateBruteForce(req.TargetIp, req.Username, req.Passwords);
        return Ok(logs);
    }

    // 2) XSS
    [HttpPost("xss")]
    public async Task<IActionResult> SimulateXss([FromBody] XssRequest req)
    {
        var log = await _threatService.SimulateFakeXss(req.TargetUrl, req.Payload);
        return Ok(log);
    }

    // 3) GET LOGS w/ optional filters
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? attackType,
        [FromQuery] string? ip)
    {
        var logs = await _logService.GetLogsAsync(startDate, endDate, attackType, ip);
        return Ok(logs);
    }
}