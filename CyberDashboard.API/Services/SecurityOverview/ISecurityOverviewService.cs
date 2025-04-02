public interface ISecurityOverviewService
{
    double GetSystemUptimeHours();
    string GetFirewallStatus();
    string GetAntivirusStatus();
    List<string> GetLoggedInUsers();
}