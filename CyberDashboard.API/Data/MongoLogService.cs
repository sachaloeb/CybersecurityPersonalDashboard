// MongoLogService.cs
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

public class MongoLogService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<SshAttemptLog> _logCollection;

    public MongoLogService(IConfiguration config)
    {
        var mongoConnection = config.GetConnectionString("MongoDB");
        var client = new MongoClient(mongoConnection);

        // Use your existing database name
        _database = client.GetDatabase("CyberDashboardNoSQL");

        // This is your original SshAttemptLogs collection
        _logCollection = _database.GetCollection<SshAttemptLog>("SshAttemptLogs");
    }

    // Keep your existing InsertLogAsync, GetAllLogsAsync, etc.

    // New: Provide a generic method to retrieve any collection
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
    
    // MongoLogService.cs
    public async Task InsertLogAsync(SshAttemptLog log)
    {
        // insert into _logCollection, which is the SshAttemptLogs collection
        await _logCollection.InsertOneAsync(log);
    }

    public async Task<List<SshAttemptLog>> GetAllLogsAsync()
    {
        // read all from _logCollection, sorted descending
        return await _logCollection
            .Find(Builders<SshAttemptLog>.Filter.Empty)
            .SortByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}