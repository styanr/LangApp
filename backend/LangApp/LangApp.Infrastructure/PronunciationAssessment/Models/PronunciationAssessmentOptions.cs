using Microsoft.CognitiveServices.Speech.PronunciationAssessment;

namespace LangApp.Infrastructure.PronunciationAssessment.Models;

public class PronunciationAssessmentOptions
{
    public string ReferenceText { get; set; }
    public GradingSystem GradingSystem { get; set; } = GradingSystem.HundredMark;
    public Granularity Granularity { get; set; } = Granularity.Phoneme;
    public bool EnableMiscue { get; set; } = true;
    public bool EnableProsody { get; set; } = true;
}
