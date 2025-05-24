using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.Pronunciation;

public record PronunciationActivityDetails : ActivityDetails
{
    public Language Language { get; private set; }
    public string ReferenceText { get; private set; }
    public bool AllowAssessment { get; private set; } = true;
    public bool AllowListening { get; private set; } = true;

    public PronunciationActivityDetails(string referenceText, Language language, bool allowAssessment = true,
        bool allowListening = true)
    {
        if (string.IsNullOrWhiteSpace(referenceText))
        {
            throw new InvalidPronunciationAssignmentDetailsException(referenceText);
        }

        ReferenceText = referenceText;
        Language = language;
        AllowAssessment = allowAssessment;
        AllowListening = allowListening;
    }
}