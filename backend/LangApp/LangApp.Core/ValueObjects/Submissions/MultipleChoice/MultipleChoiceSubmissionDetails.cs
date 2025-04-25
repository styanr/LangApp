namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceSubmissionDetails(
    List<MultipleChoiceAnswer> Answers
) : SubmissionDetails;
