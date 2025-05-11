using System.Text.Json.Serialization;
using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceAnswer
{
    [JsonPropertyName("chosenOptionIndex")]
    public int ChosenOptionIndex { get; private set; }

    public int QuestionIndex { get; private set; }

    private MultipleChoiceAnswer()
    {
    }

    public MultipleChoiceAnswer(int questionIndex, int chosenOptionIndex)
    {
        if (QuestionIndex < 0)
        {
            throw new InvalidMultipleChoiceQuestionException("Question index cannot be negative");
        }

        if (chosenOptionIndex < 0)
        {
            throw new InvalidMultipleChoiceOptionException("Chosen option index cannot be negative");
        }

        QuestionIndex = questionIndex;
        ChosenOptionIndex = chosenOptionIndex;
    }
};
