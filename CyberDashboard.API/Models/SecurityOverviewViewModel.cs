public class SecurityOverviewViewModel
{
    public required string FirewallStatus { get; set; }
    public required string AntivirusStatus { get; set; }
    public required List<string> LoggedInUsers { get; set; }
    public required double UptimeHours { get; set; }

    public SecurityOverviewViewModel()
    {
        FirewallStatus = string.Empty;
        AntivirusStatus = string.Empty;
        LoggedInUsers = new List<string>();
        UptimeHours = 0.0;
    }
}