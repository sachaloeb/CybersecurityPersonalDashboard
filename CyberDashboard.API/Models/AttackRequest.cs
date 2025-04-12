public class AttackRequest
{
    public string IP { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public List<string> Passwords { get; set; } = new();
}