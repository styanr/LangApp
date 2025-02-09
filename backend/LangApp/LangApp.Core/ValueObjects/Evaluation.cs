namespace LangApp.Core.ValueObjects;

public record Evaluation
{
    public Percentage ScorePercentage { get; init; }
    public string? Feedback { get; init; }

    public Evaluation(Percentage scorePercentage)
    {
        ScorePercentage = scorePercentage;
    }

    public Evaluation(Percentage scorePercentage, string? feedback)
    {
        ScorePercentage = scorePercentage;
        Feedback = feedback;
    }
}