using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NetworkController : ControllerBase
{
    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var status = NetworkService.GetStatus();
        return Ok(status);
    }
}