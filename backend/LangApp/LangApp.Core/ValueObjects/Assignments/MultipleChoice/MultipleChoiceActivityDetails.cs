using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceActivityDetails : ActivityDetails
{
    public MultipleChoiceActivityDetails(List<MultipleChoiceQuestion> Questions)
    {
        if (Questions.Count == 0)
        {
            throw new InvalidMultipleChoiceQuestionException("Questions cannot be null or empty.");
        }

        this.Questions = Questions;
    }

    public List<MultipleChoiceQuestion> Questions { get; init; }
}