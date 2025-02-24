namespace LangApp.Core.ValueObjects;

public record Definition
{
    public string Value { get; }

    public Definition(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Definition cannot be null or empty.");
        }

        Value = value;
    }

    public static implicit operator string(Definition definition) => definition.Value;
}