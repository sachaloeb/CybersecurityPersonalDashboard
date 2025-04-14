using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class SshAttackController : ControllerBase
{
    private readonly SshBruteForceService _sshService;

    public SshAttackController(SshBruteForceService sshService)
    {
        _sshService = sshService;
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> Simulate([FromBody] BruteForceRequest request)
    {
        var results = await _sshService.SimulateAttack(
            request.TargetIp, 
            request.Username,
            request.Passwords
        );
        return Ok(results);
    }
    
    [HttpPost("simulate-from-file")]
    public async Task<IActionResult> SimulateFromFile([FromForm] string ip, [FromForm] string username, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var passwords = new List<string>();
        // We can read line by line, but watch memory usage if the file is huge
        using var reader = new StreamReader(file.OpenReadStream());
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line))
                passwords.Add(line.Trim());
        }

        var results = await _sshService.SimulateAttack(ip, username, passwords);
        return Ok(results);
    }

    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] string source = "mongo")
    {
        return Ok(await _sshService.GetAllLogsFromMongo());
    }
}