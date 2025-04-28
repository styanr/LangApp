using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Core.ValueObjects;

namespace LangApp.Infrastructure.PronunciationAssessment;

public class PronunciationAssessmentService : IPronunciationAssessmentService
{
    public Task<SubmissionGrade> Assess(string fileUri, string referenceText, Language language)
    {
        Console.WriteLine($"{fileUri}, {referenceText}, {language.Value}");

        return Task.FromResult(new SubmissionGrade(new Percentage(100), "Good job!"));
    }
}
