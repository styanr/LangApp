using LangApp.Core.Exceptions.ValueObjects.Submissions.MultipleChoice;

namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceSubmissionDetails : SubmissionDetails
{
    public MultipleChoiceSubmissionDetails(List<MultipleChoiceAnswer> Answers)
    {
        if (Answers.Select(a => a.QuestionIndex).Distinct().Count() != Answers.Count)
        {
            throw new DuplicateQuestionIndexInSubmissionException();
        }

        this.Answers = Answers;
    }

    public List<MultipleChoiceAnswer> Answers { get; init; }
}