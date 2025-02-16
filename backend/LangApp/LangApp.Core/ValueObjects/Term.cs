namespace LangApp.Core.ValueObjects;

public record Term
{
    public string Value { get; }

    public Term(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value), "Expression cannot be null or empty.");
        }

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}