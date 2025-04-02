using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management; // For WMI on Windows
using System.Linq;

public class SecurityOverviewServiceWindows : ISecurityOverviewService
{
    public double GetSystemUptimeHours()
    {
        // Uses Win32_OperatingSystem (WMI)
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject mo in searcher.Get())
            {
                DateTime lastBootUp = ManagementDateTimeConverter
                    .ToDateTime(mo["LastBootUpTime"].ToString());
                var uptime = DateTime.Now - lastBootUp;
                return Math.Round(uptime.TotalHours, 2);
            }
        }
        catch
        {
            // fallback
        }
        return 0.0;
    }

    public string GetFirewallStatus()
    {
        // Checks if MpsSvc (Windows Firewall Service) is running
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_Service WHERE Name='MpsSvc'");
            foreach (ManagementObject service in searcher.Get())
            {
                string state = service["State"]?.ToString() ?? "";
                return state.Equals("Running", StringComparison.OrdinalIgnoreCase)
                    ? "Enabled"
                    : "Disabled";
            }
        }
        catch
        {
            // ignore
        }
        return "Error checking firewall status";
    }

    public string GetAntivirusStatus()
    {
        // Query root\SecurityCenter2 -> AntivirusProduct
        try
        {
            using var searcher = new ManagementObjectSearcher(
                @"root\SecurityCenter2",
                "SELECT * FROM AntivirusProduct");
            var results = searcher.Get();
            var avNames = new List<string>();

            foreach (ManagementObject obj in results)
            {
                string name = obj["displayName"]?.ToString();
                if (!string.IsNullOrEmpty(name))
                {
                    avNames.Add(name);
                }
            }

            if (avNames.Count > 0)
                return "Antivirus installed: " + string.Join(", ", avNames);

            return "No antivirus found or not reported via WMI";
        }
        catch (Exception ex)
        {
            return $"Error checking antivirus: {ex.Message}";
        }
    }

    public List<string> GetLoggedInUsers()
    {
        var users = new List<string>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LoggedOnUser");
            foreach (ManagementObject logonObj in searcher.Get())
            {
                var accountPath = logonObj["Antecedent"]?.ToString();
                if (!string.IsNullOrEmpty(accountPath))
                {
                    using var account = new ManagementObject(accountPath);
                    string domain = account["Domain"]?.ToString();
                    string user = account["Name"]?.ToString();

                    if (!string.IsNullOrEmpty(user))
                        users.Add($"{domain}\\{user}");
                }
            }
        }
        catch (Exception ex)
        {
            users.Add($"Error: {ex.Message}");
        }
        return users.Distinct().ToList(); // Avoid duplicates
    }
}