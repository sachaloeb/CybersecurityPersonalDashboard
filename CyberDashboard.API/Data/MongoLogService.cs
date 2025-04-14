using MongoDB.Driver;

public class MongoLogService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ThreatLog> _logCollection;

    public MongoLogService(IConfiguration config)
    {
        var mongoConnection = config.GetConnectionString("MongoDB");
        var client = new MongoClient(mongoConnection);
        var database = client.GetDatabase("CyberDashboardNoSQL");

        _logCollection = database.GetCollection<ThreatLog>("ThreatLogs");
    }

    public async Task InsertLogAsync(ThreatLog log)
    {
        await _logCollection.InsertOneAsync(log);
    }

    public async Task<List<ThreatLog>> GetAllLogsAsync()
    {
        return await _logCollection
            .Find(Builders<ThreatLog>.Filter.Empty)
            .SortByDescending(l => l.Timestamp)
            .ToListAsync();
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}