using MongoDB.Bson;
public class ZapScanLog
{
    public ObjectId Id { get; set; }
    public string Url        { get; set; }   // target that was scanned
    public string MaxRisk    { get; set; }   // High / Medium / …
    public IEnumerable<string> AlertIds { get; set; } // e.g. “Missing CSP…”
    public DateTime Timestamp { get; set; }  // UTC
}