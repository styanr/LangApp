using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.FillInTheBlank;

public record FillInTheBlankAnswer
{
    public FillInTheBlankAnswer(List<string> acceptableAnswers)
    {
        foreach (var a in acceptableAnswers
                     .Select((item, index) => (item, index))
                     .Where(tuple => string.IsNullOrWhiteSpace(tuple.item)))
        {
            throw new InvalidFillInTheBlankAnswerException(a.index);
        }

        AcceptableAnswers = acceptableAnswers;
    }

    public List<string> AcceptableAnswers { get; }
};