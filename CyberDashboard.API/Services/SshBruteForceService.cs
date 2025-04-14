using Renci.SshNet;

public class SshBruteForceService
{
    private readonly AppDbContext _db; // Postgres
    private readonly MongoLogService _mongo; // Mongo

    public SshBruteForceService(AppDbContext db, MongoLogService mongo)
    {
        _db = db;
        _mongo = mongo;
    }

    public async Task<List<ThreatLog>> SimulateAttack(string ip, string username, List<string> passwords)
    {
        var results = new List<ThreatLog>();

        foreach (var pwd in passwords)
        {
            var success = false;
            try
            {
                using var client = new SshClient(ip, username, pwd);
                client.Connect();
                success = client.IsConnected;
                client.Disconnect();
            }
            catch
            {
                success = false;
            }

            var log = new ThreatLog
            {
                Id = null, // auto from Mongo
                Timestamp = DateTime.UtcNow,
                AttackType = "BRUTE_FORCE",
                Target = ip,
                Result = success,
                ExtraInfo = $"user={username}, password={pwd}"
            };

            // Insert in Mongo
            await _mongo.InsertLogAsync(log);
            results.Add(log);
        }

        // OPTIONAL: If you want to store in Postgres too:
        // _db.SshAttemptLogs.AddRange(results);
        // await _db.SaveChangesAsync();

        return results;
    }

    // If your GET logs route is now primarily from Mongo
    public async Task<List<ThreatLog>> GetAllLogsFromMongo()
    {
        return await _mongo.GetAllLogsAsync();
    }
}