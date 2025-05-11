using LangApp.Core.Exceptions;

namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceSubmissionDetails : SubmissionDetails
{
    public MultipleChoiceSubmissionDetails(List<MultipleChoiceAnswer> Answers)
    {
        if (Answers.Select(a => a.QuestionIndex).Distinct().Count() != Answers.Count)
        {
            throw new LangAppException("Answers must contain unique question indexes");
        }

        this.Answers = Answers;
    }

    public List<MultipleChoiceAnswer> Answers { get; init; }
}
