using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// Stores brute-force logs in MongoDB (bulk data).
/// </summary>
public class MongoLogService
{
    private readonly IMongoCollection<SshAttemptLog> _logCollection;

    public MongoLogService(IConfiguration config)
    {
        // 1) Grab your MongoDB connection string from config
        var mongoConnection = config.GetConnectionString("MongoDB");

        // 2) Initialize the client and database
        var client = new MongoClient(mongoConnection);
        var database = client.GetDatabase("CyberDashboardNoSQL"); 
        // or any db name you prefer

        // 3) Create the collection for logs
        _logCollection = database.GetCollection<SshAttemptLog>("SshAttemptLogs");
    }

    public async Task InsertLogAsync(SshAttemptLog log)
    {
        await _logCollection.InsertOneAsync(log);
    }

    public async Task<List<SshAttemptLog>> GetAllLogsAsync()
    {
        // Sort by descending Timestamp
        return await _logCollection
            .Find(Builders<SshAttemptLog>.Filter.Empty)
            .SortByDescending(l => l.Timestamp)
            .ToListAsync();
    }
}