namespace LangApp.Core.ValueObjects;

public record SubmissionGrade
{
    public Percentage ScorePercentage { get; init; }
    public string? Feedback { get; init; }

    private SubmissionGrade()
    {
    }

    public SubmissionGrade(Percentage scorePercentage)
    {
        ScorePercentage = scorePercentage;
    }

    public SubmissionGrade(Percentage scorePercentage, string? feedback)
    {
        ScorePercentage = scorePercentage;
        Feedback = feedback;
    }
}
