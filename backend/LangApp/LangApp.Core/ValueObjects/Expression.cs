namespace LangApp.Core.ValueObjects;

public record Expression
{
    public string Value { get; }

    public Expression(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Expression cannot be null or empty.");
        }

        Value = value;
    }
}