using LangApp.Core.ValueObjects;

namespace LangApp.Core.Services.PronunciationAssessment;

public interface IPronunciationAssessmentService
{
    Task<Percentage> Assess(string fileUri, string referenceText, Language language);
}