using System.Security.Cryptography;
using System.Text;
using Zxcvbn;

public static class PasswordService
{
    public static PasswordEvaluation Evaluate(string password)
    {
        // Evaluate with Zxcvbn
        var zxcvbnResult = Zxcvbn.Core.EvaluatePassword(password);

        // Hash the password (SHA-256)
        string hashed = HashPasswordSha256(password);

        // Convert numeric score to a label
        string label = GetStrengthLabel(zxcvbnResult.Score);

        return new PasswordEvaluation
        {
            HashedPassword = hashed,
            Score = zxcvbnResult.Score,
            StrengthLabel = label,
            Feedback = zxcvbnResult.Feedback.Suggestions != null 
                ? string.Join(", ", zxcvbnResult.Feedback.Suggestions) 
                : "No feedback"
        };
    }

    // Helper method to do SHA-256 hashing
    private static string HashPasswordSha256(string pwd)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pwd));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    // Map numeric score to an easy-to-read label
    private static string GetStrengthLabel(int score)
    {
        return score switch
        {
            0 => "Very Weak",
            1 => "Weak",
            2 => "Medium",
            3 => "Strong",
            4 => "Very Strong",
            _ => "Unknown"
        };
    }
}