public class SshAttemptLog
{
    public int Id { get; set; }
    public required string IPAddress { get; set; }
    public string Username { get; set; }
    public string Password { get; set; } // Optional - mask on frontend or omit
    public bool Success { get; set; }
    public DateTime Timestamp { get; set; }
}