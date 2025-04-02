using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class SecurityOverviewServiceMac : ISecurityOverviewService
{
    public double GetSystemUptimeHours()
    {
        try
        {
            string output = RunProcess("ps", new[] { "-p", "1", "-o", "etime=" }).Trim();
        
            TimeSpan uptime;

            if (output.Contains("-"))
            {
                // Format: d-HH:MM:SS or d-HH:MM
                var parts = output.Split('-');
                int days = int.Parse(parts[0]);
                string[] timeParts = parts[1].Split(':');
                int hours = int.Parse(timeParts[0]);
                int minutes = int.Parse(timeParts[1]);
                int seconds = timeParts.Length > 2 ? int.Parse(timeParts[2]) : 0;

                uptime = new TimeSpan(days, hours, minutes, seconds);
            }
            else
            {
                // Format: HH:MM:SS or MM:SS
                string[] timeParts = output.Split(':');
                int hours = 0, minutes = 0, seconds = 0;
                if (timeParts.Length == 3)
                {
                    hours = int.Parse(timeParts[0]);
                    minutes = int.Parse(timeParts[1]);
                    seconds = int.Parse(timeParts[2]);
                }
                else if (timeParts.Length == 2)
                {
                    minutes = int.Parse(timeParts[0]);
                    seconds = int.Parse(timeParts[1]);
                }
                uptime = new TimeSpan(hours, minutes, seconds);
            }

            return uptime.TotalHours;
        }
        catch
        {
            return 0.0;
        }
    }

    public string GetFirewallStatus()
    {
        // "/usr/libexec/ApplicationFirewall/socketfilterfw --getglobalstate"
        try
        {
            string output = RunProcess("/usr/libexec/ApplicationFirewall/socketfilterfw",
                                       new[] { "--getglobalstate" });
            if (output.Contains("enabled", StringComparison.OrdinalIgnoreCase))
                return "Enabled";
            if (output.Contains("disabled", StringComparison.OrdinalIgnoreCase))
                return "Disabled";
            return "Unknown";
        }
        catch (Exception ex)
        {
            return $"Error or Not Installed: {ex.Message}";
        }
    }

    public string GetAntivirusStatus()
    {
        // Check for known AV processes
        var knownAntivirus = new[]
        {
            "symdaemon", // Symantec
            "ScanManager" // McAfee
        };

        foreach (var av in knownAntivirus)
        {
            try
            {
                string output = RunProcess("pgrep", new[] { av });
                if (!string.IsNullOrWhiteSpace(output))
                {
                    return $"Antivirus running: {av}";
                }
            }
            catch
            {
                // Non-zero exit means not found
            }
        }
        return "No antivirus found";
    }

    public List<string> GetLoggedInUsers()
    {
        var users = new List<string>();
        try
        {
            string output = RunProcess("who", Array.Empty<string>());
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 0)
                    users.Add(tokens[0]);
            }
        }
        catch (Exception ex)
        {
            users.Add($"Error: {ex.Message}");
        }
        return users.Distinct().ToList();
    }

    // --------------------------------------------------
    // HELPER: run a child process
    // --------------------------------------------------
    private string RunProcess(string fileName, string[] args)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        foreach (var arg in args)
            startInfo.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return output;
    }
}