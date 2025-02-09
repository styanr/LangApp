using LangApp.Core.Exceptions.Exercises;

namespace LangApp.Core.ValueObjects.Exercises;

public record MultipleChoiceQuestion
{
    private readonly List<MultipleChoiceOption> _options;

    public string Question { get; private set; }
    public IReadOnlyList<MultipleChoiceOption> Options => _options.AsReadOnly();
    public string CorrectOption { get; private set; }

    public MultipleChoiceQuestion(string question, List<MultipleChoiceOption> options, string correctOption)
    {
        if (string.IsNullOrEmpty(Question))
        {
            throw new InvalidMultipleChoiceQuestionException("Question text cannot be empty");
        }

        if (options.All(o => o.OptionKey != CorrectOption))
        {
            throw new InvalidMultipleChoiceQuestionException("Correct option missing in the list of options");
        }

        Question = question;
        _options = options;
        CorrectOption = correctOption;
    }
};