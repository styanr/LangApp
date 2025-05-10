namespace LangApp.Core.ValueObjects;

public record SubmissionGrade
{
    // public Percentage ScorePercentage { get; init; }
    public double ScorePercentage { get; init; }
    public string? Feedback { get; init; }

    private SubmissionGrade()
    {
    }

    public SubmissionGrade(double scorePercentage)
    {
        // ScorePercentage = new Percentage(scorePercentage);

        ScorePercentage = scorePercentage;
    }

    public SubmissionGrade(double scorePercentage, string? feedback)
    {
        // ScorePercentage = new Percentage(scorePercentage);
        ScorePercentage = scorePercentage;
        Feedback = feedback;
    }
}
