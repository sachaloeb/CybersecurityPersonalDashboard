using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PasswordController : ControllerBase
{
    [HttpPost("evaluate")]
    public IActionResult Evaluate([FromBody] string password)
    {
        var result = PasswordService.Evaluate(password);
        return Ok(result);
    }
}