using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class SshAttemptLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string IPAddress { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
}