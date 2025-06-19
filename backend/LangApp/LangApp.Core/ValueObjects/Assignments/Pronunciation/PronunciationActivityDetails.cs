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
        if (!IsValidReferenceText(referenceText))
        {
            throw new InvalidPronunciationActivityReferenceTextException(referenceText);
        }

        if (!IsValidReferenceTextLength(referenceText))
        {
            throw new PronunciationActivityReferenceTextTooLongException(referenceText.Length, 1000);
        }

        ReferenceText = referenceText;
        Language = language;
        AllowAssessment = allowAssessment;
        AllowListening = allowListening;
    }

    private static bool IsValidReferenceText(string referenceText)
    {
        return !string.IsNullOrWhiteSpace(referenceText);
    }

    private static bool IsValidReferenceTextLength(string referenceText)
    {
        const int maxLength = 1000;
        return referenceText.Length <= maxLength;
    }
}