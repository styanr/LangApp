using LangApp.Core.Exceptions.Assignments;

namespace LangApp.Core.ValueObjects.Assignments.MultipleChoice;

public record MultipleChoiceOption
{
    public string OptionDescription { get; private set; }

    public MultipleChoiceOption(string optionDescription)
    {
        if (string.IsNullOrEmpty(optionDescription))
        {
            throw new InvalidMultipleChoiceOptionException("Option description cannot be null");
        }

        OptionDescription = optionDescription;
    }
};