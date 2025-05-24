using System.Text.Json.Serialization;
using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

// Json attributes are used here for the deserialization.

public record MultipleChoiceQuestion
{
    [JsonPropertyName("question")] public string Question { get; }
    [JsonPropertyName("options")] public List<MultipleChoiceOption> Options { get; }

    [JsonPropertyName("correctOptionIndex")]
    public int CorrectOptionIndex { get; }

    public MultipleChoiceQuestion(string question, List<MultipleChoiceOption> options, int correctOptionIndex)
    {
        if (string.IsNullOrWhiteSpace(question))
        {
            throw new InvalidMultipleChoiceQuestionException("Question text cannot be empty");
        }

        if (options.Count == 0)
        {
            throw new InvalidMultipleChoiceQuestionException("At least one option must be provided");
        }

        if (correctOptionIndex < 0 || correctOptionIndex >= options.Count)
        {
            throw new InvalidMultipleChoiceQuestionException("Correct option index is out of bounds");
        }

        Question = question;
        Options = options;
        CorrectOptionIndex = correctOptionIndex;
    }
}
