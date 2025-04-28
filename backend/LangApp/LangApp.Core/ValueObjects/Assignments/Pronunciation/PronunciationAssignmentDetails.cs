using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.Pronunciation;

public record PronunciationAssignmentDetails : AssignmentDetails
{
    public Language Language { get; private set; }
    public string ReferenceText { get; private set; }

    public PronunciationAssignmentDetails(string referenceText, Language language)
    {
        if (string.IsNullOrWhiteSpace(referenceText))
        {
            throw new InvalidPronunciationAssignmentDetailsException(referenceText);
        }

        ReferenceText = referenceText;
        Language = language;
    }
}
