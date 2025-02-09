using LangApp.Core.Exceptions.Exercises;

namespace LangApp.Core.ValueObjects.Exercises;

public record MultipleChoiceOption
{
    public string OptionKey { get; private set; }
    public string OptionDescription { get; private set; }

    public MultipleChoiceOption(string optionKey, string optionDescription)
    {
        if (string.IsNullOrEmpty(optionKey))
        {
            throw new InvalidMultipleChoiceOptionException("Option key cannot be null");
        }

        if (string.IsNullOrEmpty(optionDescription))
        {
            throw new InvalidMultipleChoiceOptionException("Option description cannot be null");
        }

        OptionKey = optionKey;
        OptionDescription = optionDescription;
    }
};