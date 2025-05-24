using LangApp.Core.Exceptions;

namespace LangApp.Core.ValueObjects.Submissions.Question;

public record QuestionSubmissionDetails : SubmissionDetails
{
    public string Answer { get; private set; }

    public QuestionSubmissionDetails(string answer)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new LangAppException("Answer cannot be empty");
        }

        Answer = answer;
    }
}
