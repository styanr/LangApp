using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceQuestion
{
    private readonly List<MultipleChoiceOption> _options;

    public string Question { get; }
    public IReadOnlyList<MultipleChoiceOption> Options => _options.AsReadOnly();
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
        _options = options;
        CorrectOptionIndex = correctOptionIndex;
    }
}