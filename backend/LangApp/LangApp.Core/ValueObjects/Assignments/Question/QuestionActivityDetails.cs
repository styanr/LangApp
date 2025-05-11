using LangApp.Core.Exceptions;

namespace LangApp.Core.ValueObjects.Assignments.Question;

public record QuestionActivityDetails : ActivityDetails
{
    public string Question { get; init; }
    public List<string> Answers { get; init; }
    public int MaxLength { get; init; } = 100;

    public QuestionActivityDetails(string question, List<string> answers, int maxLength)
    {
        if (maxLength is < 0 or > 100)
        {
            throw new LangAppException("Max length must be between 0 and 100");
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new LangAppException("Question cannot be empty");
        }

        if (answers.Count < 2)
        {
            throw new LangAppException("Question must have at least 2 answers");
        }

        foreach (var answer in answers)
        {
            if (answer.Length > MaxLength)
            {
                throw new LangAppException($"Answer cannot be longer than {MaxLength} characters");
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new LangAppException("Answer cannot be empty");
            }
        }

        Question = question;
        Answers = answers;
        MaxLength = maxLength;
    }
}
