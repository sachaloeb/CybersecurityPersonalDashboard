using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class ThreatLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime Timestamp { get; set; }
    public string Target { get; set; } = "";
    public string AttackType { get; set; } = "";
    public bool Result { get; set; }

    public string ExtraInfo { get; set; } = ""; 

    // ✅ Add this if you want IP-based filtering
    public string IP { get; set; } = "";
}