using Zxcvbn;

public static class PasswordService
{
    public static PasswordEvaluation Evaluate(string password)
    {
        //var zxcvbn = new Zxcvbn(); // Correct class from zxcvbn-core
        var result = Zxcvbn.Core.EvaluatePassword(password);

        return new PasswordEvaluation
        {
            Score = result.Score,
            Feedback = result.Feedback.Suggestions != null ? string.Join(", ", result.Feedback.Suggestions) : "No feedback"
        };
    }
}