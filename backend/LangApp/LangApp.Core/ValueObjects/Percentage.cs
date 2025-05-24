namespace LangApp.Core.ValueObjects;

public record Percentage
{
    public double Value { get; private set; }

    public Percentage(double value)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Percentage must be between 0 and 100 inclusive");
        }

        Value = Math.Round(value, 2);
    }
}
