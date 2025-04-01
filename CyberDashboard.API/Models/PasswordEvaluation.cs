public class PasswordEvaluation
{
    public string HashedPassword { get; set; } = string.Empty;
    public int Score { get; set; }
    public string StrengthLabel { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
}