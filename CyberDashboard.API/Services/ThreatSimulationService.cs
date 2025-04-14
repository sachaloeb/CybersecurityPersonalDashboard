using Renci.SshNet;

public class ThreatSimulationService
{
    private readonly ThreatLogService _logService;

    public ThreatSimulationService(ThreatLogService logService)
    {
        _logService = logService;
    }

    // 1) Brute Force
    public async Task<List<ThreatLog>> SimulateBruteForce(string targetIp, string username, List<string> passwords)
    {
        var logs = new List<ThreatLog>();

        foreach (var pwd in passwords)
        {
            var success = false;
            try
            {
                using var client = new SshClient(targetIp, username, pwd);
                client.Connect();
                success = client.IsConnected;
                client.Disconnect();
            }
            catch
            {
                success = false;
            }

            var logEntry = new ThreatLog
            {
                Timestamp = DateTime.UtcNow,
                Target = targetIp,
                AttackType = "BRUTE_FORCE",
                Result = success,
                IP = "" // or the user's machine IP if you want
            };
            await _logService.InsertLogAsync(logEntry);
            logs.Add(logEntry);
        }

        return logs;
    }

    // 2) Fake XSS
    public async Task<ThreatLog> SimulateFakeXss(string targetUrl, string payload)
    {
        // In real life, you'd try to inject <script> etc. into the target
        // We'll just "simulate" it
        var success = payload.Contains("<script"); // silly check or always false

        var logEntry = new ThreatLog
        {
            Timestamp = DateTime.UtcNow,
            Target = targetUrl,
            AttackType = "XSS",
            Result = success
        };
        await _logService.InsertLogAsync(logEntry);
        return logEntry;
    }
}