namespace LangApp.Core.ValueObjects;

public record DictionaryTitle
{
    public string Value { get; }

    public DictionaryTitle(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Title cannot be null or empty.");
        }

        Value = value;
    }
}