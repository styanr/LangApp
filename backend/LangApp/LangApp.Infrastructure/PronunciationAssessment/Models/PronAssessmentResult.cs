namespace LangApp.Infrastructure.PronunciationAssessment.Models;

public class PronAssessmentResult
{
    public double AccuracyScore { get; set; }
    public double PronunciationScore { get; set; }
    public double CompletenessScore { get; set; }
    public double FluencyScore { get; set; }
    public double ProsodyScore { get; set; }
    public List<Word> Words { get; set; } = new List<Word>();
}

public class Word
{
    public string WordText { get; set; }
    public string ErrorType { get; set; }
    public double AccuracyScore { get; set; }

    public Word(string wordText, string errorType, double accuracyScore = 0)
    {
        WordText = wordText;
        ErrorType = errorType;
        AccuracyScore = accuracyScore;
    }
}
