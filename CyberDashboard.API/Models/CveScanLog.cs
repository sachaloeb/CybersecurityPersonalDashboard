using MongoDB.Bson;
public class CveScanLog
{
    public ObjectId Id { get; set; }
    public string Url         { get; set; }
    public IEnumerable<string> CveIds  { get; set; }  // CVE‑2025‑1234 …
    public DateTime Timestamp { get; set; }           // UTC
}