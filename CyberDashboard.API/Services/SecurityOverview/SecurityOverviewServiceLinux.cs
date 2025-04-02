using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class SecurityOverviewServiceLinux : ISecurityOverviewService
{
    public double GetSystemUptimeHours()
    {
        // Try reading /proc/uptime
        try
        {
            string[] lines = File.ReadAllLines("/proc/uptime");
            if (lines.Length > 0)
            {
                string[] parts = lines[0].Split(' ');
                if (parts.Length > 0 && double.TryParse(parts[0], out double seconds))
                {
                    return Math.Round(seconds / 3600, 2);
                }
            }
        }
        catch
        {
            // If reading /proc/uptime fails, fallback or return 0
        }

        // Fallback approach:
        return 0.0;
    }

    public string GetFirewallStatus()
    {
        // We'll call "ufw status" and see if it includes "active"
        try
        {
            string output = RunProcess("sudo", new[] { "ufw", "status" });
            if (output.ToLower().Contains("active"))
                return "Enabled";
            return "Disabled";
        }
        catch (Exception ex)
        {
            return $"Error or Not Installed: {ex.Message}";
        }
    }

    public string GetAntivirusStatus()
    {
        // Check for known processes (clamd, freshclam, etc.)
        var knownAntivirus = new[] { "clamd", "freshclam", "sav-protect" };
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
                // pgrep returns non-zero if process not found
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
            // Example line: "john    tty7   2023-10-12 10:01 (:0)"
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