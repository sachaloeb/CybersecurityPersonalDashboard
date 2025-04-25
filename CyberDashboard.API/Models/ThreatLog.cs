using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class ThreatLog
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public DateTime   Timestamp  { get; set; } = DateTime.UtcNow;
    public string     Target     { get; set; } = "";
    public AttackKind AttackType { get; set; }           // <- enum
    public bool       Result     { get; set; }
    public string     ExtraInfo  { get; set; } = "";
    public string     IP         { get; set; } = "";
}