// Services/ThreatLogService.cs
using Renci.SshNet;
using MongoDB.Driver;

public class ThreatLogService
{
    private readonly IMongoCollection<ThreatLog> _collection;

    public ThreatLogService(IConfiguration config)
    {
        var mongoConn = config.GetConnectionString("MongoDB"); // e.g. "mongodb://localhost:27017"
        var client = new MongoClient(mongoConn);
        var db = client.GetDatabase("CyberDashboardNoSQL");
        _collection = db.GetCollection<ThreatLog>("ThreatLogs");
    }

    public async Task InsertLogAsync(ThreatLog log)
    {
        await _collection.InsertOneAsync(log);
    }

    public async Task<List<ThreatLog>> GetLogsAsync(
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? attackType = null,
        string? ip = null)
    {
        // Build a dynamic filter
        var filterBuilder = Builders<ThreatLog>.Filter;
        var filter = filterBuilder.Empty;

        // Filter by date range
        if (startDate.HasValue)
            filter &= filterBuilder.Gte(l => l.Timestamp, startDate.Value);
        if (endDate.HasValue)
            filter &= filterBuilder.Lte(l => l.Timestamp, endDate.Value);

        // Filter by attack type
        if (!string.IsNullOrEmpty(attackType)) 
            filter &= filterBuilder.Eq(nameof(ThreatLog.AttackType), attackType);  

        // Filter by IP
        if (!string.IsNullOrEmpty(ip))
            filter &= filterBuilder.Eq(l => l.IP, ip);

        // Sort descending by Timestamp
        return await _collection
            .Find(filter)
            .SortByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}