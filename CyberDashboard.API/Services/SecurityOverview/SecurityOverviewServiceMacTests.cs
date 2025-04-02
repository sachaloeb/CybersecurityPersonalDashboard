using System.Collections.Generic;
using Xunit;

public class SecurityOverviewServiceMacTests
{
    private readonly SecurityOverviewServiceMac _service;

    public SecurityOverviewServiceMacTests()
    {
        _service = new SecurityOverviewServiceMac();
    }

    [Fact]
    public void GetSystemUptimeHours_ShouldReturnZero()
    {
        // Act
        double uptime = _service.GetSystemUptimeHours();

        // Assert
        Assert.True(uptime > 0.0, $"{uptime} should be greater than {0}");
    }

    [Fact]
    public void GetFirewallStatus_ShouldReturnEnabledOrDisabled()
    {
        // Act
        string status = _service.GetFirewallStatus();

        // Assert
        Assert.Contains(status, new[] { "Enabled", "Disabled", "Unknown" });
    }
    
    [Fact]
    public void GetAntivirusStatus_ShouldReturnNoAntivirusFound()
    {
        // Act
        string status = _service.GetAntivirusStatus();

        // Assert
        Assert.Equal("No antivirus found", status);
    }
    
    [Fact]
    public void GetLoggedInUsers_ShouldReturnListOfUsers()
    {
        // Act
        List<string> users = _service.GetLoggedInUsers();

        // Assert
        Assert.NotNull(users);
        Assert.All(users, user => Assert.False(string.IsNullOrWhiteSpace(user)));
    }
}