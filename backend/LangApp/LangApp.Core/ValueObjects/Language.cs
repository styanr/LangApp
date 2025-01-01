using LangApp.Core.Common;
using LangApp.Core.Exceptions.Language;

namespace LangApp.Core.ValueObjects;

public record Language
{
    public string Value { get; private set; }

    public Language(string value)
    {
        // probably needs better validation?
        if (string.IsNullOrEmpty(value))
        {
            throw new LanguageEmptyException();
        }

        Value = value;
    }
}