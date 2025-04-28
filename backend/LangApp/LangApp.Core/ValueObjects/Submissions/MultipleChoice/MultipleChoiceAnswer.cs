using System.Text.Json.Serialization;
using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Submissions.MultipleChoice;

public record MultipleChoiceAnswer
{
    [JsonPropertyName("chosenOptionIndex")]
    public int ChosenOptionIndex { get; private set; }

    private MultipleChoiceAnswer()
    {
    }

    public MultipleChoiceAnswer(int chosenOptionIndex)
    {
        if (chosenOptionIndex < 0)
        {
            throw new InvalidMultipleChoiceOptionException("Chosen option index cannot be negative");
        }

        ChosenOptionIndex = chosenOptionIndex;
    }
};
