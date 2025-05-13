namespace LangApp.Infrastructure.PronunciationAssessment.Models;

public class SpeechServiceConfig
{
    public string SubscriptionKey { get; set; }
    public string Region { get; set; }
    public string Language { get; set; } = "en-US";
}
