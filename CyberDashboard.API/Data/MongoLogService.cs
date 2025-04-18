using MongoDB.Driver;

public class MongoLogService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<ThreatLog> _logCollection;
    public IMongoCollection<PortScanLog> PortScanLogs;
    public IMongoCollection<ZapScanLog> ZapScanLogs   => _database.GetCollection<ZapScanLog>("ZapScanLogs");
    public IMongoCollection<CveScanLog> CveScanLogs   => _database.GetCollection<CveScanLog>("CveScanLogs");

    public Task InsertZapAsync(ZapScanLog log) => ZapScanLogs.InsertOneAsync(log);
    public Task InsertCveAsync(CveScanLog log) => CveScanLogs.InsertOneAsync(log);

    public MongoLogService(IConfiguration config)
    {
        var mongoConnection = config.GetConnectionString("MongoDB");
        var client = new MongoClient(mongoConnection);
        _database = client.GetDatabase("CyberDashboardNoSQL");
        _logCollection     = _database.GetCollection<ThreatLog>("ThreatLogs");
        PortScanLogs       = _database.GetCollection<PortScanLog>("PortScanLogs");
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