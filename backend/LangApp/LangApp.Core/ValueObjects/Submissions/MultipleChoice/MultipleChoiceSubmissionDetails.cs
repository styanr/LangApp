namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceSubmissionDetails : SubmissionDetails
{
    private readonly List<MultipleChoiceAnswer> _answers;
    public IReadOnlyList<MultipleChoiceAnswer> Answers => _answers.AsReadOnly();

    public MultipleChoiceSubmissionDetails(List<MultipleChoiceAnswer> answers)
    {
        _answers = answers;
    }
}