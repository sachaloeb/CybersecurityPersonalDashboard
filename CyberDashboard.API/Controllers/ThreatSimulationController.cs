using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Renci.SshNet;

// Controllers/ThreatSimulationController.cs
[ApiController]
[Route("api/threatsimulation")]
public class ThreatSimulationController : ControllerBase
{
    private readonly ThreatSimulationService _svc;
    private readonly MongoLogService         _mongo;
    public ThreatSimulationController(ThreatSimulationService svc, MongoLogService mongo)
    { _svc = svc; _mongo = mongo; }

    [HttpPost("brute")]
    public async Task<IActionResult> Brute([FromBody] BruteForceRequest r) =>
        Ok(await _svc.SimulateBruteAsync(r));

    [HttpPost("xss")]
    public async Task<IActionResult> Xss([FromBody] XssRequest r) =>
        Ok(await _svc.SimulateXssAsync(r));

    [HttpGet("logs")]
    public async Task<IActionResult> Logs
    (
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] AttackKind? attackType,
        [FromQuery] string? ip
    )
    {
        var f = Builders<ThreatLog>.Filter.Empty;
        if (startDate is not null)  f &= Builders<ThreatLog>.Filter.Gte(l => l.Timestamp, startDate);
        if (endDate   is not null)  f &= Builders<ThreatLog>.Filter.Lte(l => l.Timestamp, endDate);
        if (attackType!= null)      f &= Builders<ThreatLog>.Filter.Eq (l => l.AttackType, attackType);
        if (!string.IsNullOrEmpty(ip)) f &= Builders<ThreatLog>.Filter.Eq(l => l.IP, ip);

        var logs = await _mongo.GetCollection<ThreatLog>("ThreatLogs")
            .Find(f)
            .SortByDescending(l => l.Timestamp)
            .ToListAsync();
        return Ok(logs);
    }
}