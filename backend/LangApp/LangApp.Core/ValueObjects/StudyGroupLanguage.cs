using LangApp.Core.Common;

namespace LangApp.Core.ValueObjects;

public record StudyGroupLanguage
{
    public string Value { get; private set; }

    public StudyGroupLanguage(string value)
    {
        // probably needs better validation?
        if (string.IsNullOrEmpty(value))
        {
            throw new LanguageEmptyException();
        }

        Value = value;
    }
}