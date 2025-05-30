using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.ValueObjects.Question;

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
            throw new InvalidQuestionMaxLengthException(maxLength);
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new EmptyQuestionTextException();
        }

        if (answers.Count < 1)
        {
            throw new NoAnswersProvidedForQuestionException();
        }

        foreach (var answer in answers)
        {
            if (answer.Length > MaxLength)
            {
                throw new AnswerTooLongException(answer, MaxLength);
            }

            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new EmptyAnswerException();
            }
        }

        Question = question;
        Answers = answers;
        MaxLength = maxLength;
    }
}