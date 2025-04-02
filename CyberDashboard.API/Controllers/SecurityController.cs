using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

[ApiController]
[Route("/api/[controller]")]
public class SecurityController : ControllerBase
{
    private readonly ISecurityOverviewService _securityOverviewService= SecurityOverviewServiceFactory.Create();

    [HttpGet("security-overview")]
    public ActionResult<SecurityOverviewViewModel> Overview()
    {
        var overview = new SecurityOverviewViewModel
        {
            UptimeHours = _securityOverviewService.GetSystemUptimeHours(),
            FirewallStatus = _securityOverviewService.GetFirewallStatus(),
            AntivirusStatus = _securityOverviewService.GetAntivirusStatus(),
            LoggedInUsers = _securityOverviewService.GetLoggedInUsers()
        };
        return Ok(overview);
    }
}