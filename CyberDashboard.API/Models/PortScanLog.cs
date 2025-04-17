using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class PortScanLog
{
    [BsonId]                       public ObjectId Id        { get; set; }
    public string                  Host       { get; set; } = string.Empty;
    public int                     StartPort  { get; set; }
    public int                     EndPort    { get; set; }
    public DateTime                StartedAt  { get; set; } = DateTime.UtcNow;
    public DateTime?               EndedAt    { get; set; }
    public TimeSpan?               Duration   { get; set; }
    public ScanStatus              Status     { get; set; } = ScanStatus.Pending;

    // store the finished result (truncated here for brevity)
    public List<PortStatusInfo>?   Ports      { get; set; }
    public bool?                   IsHostUp   { get; set; }
}