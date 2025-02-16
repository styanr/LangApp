namespace LangApp.Core.ValueObjects;

public record LexiconTitle
{
    public string Value { get; }

    private LexiconTitle()
    {
    }

    public LexiconTitle(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Title cannot be null or empty.");
        }

        Value = value;
    }
}