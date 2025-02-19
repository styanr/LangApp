namespace LangApp.Core.ValueObjects;

public record UserFullName
{
    public UserFullName(string FirstName, string LastName)
    {
        this.FirstName = FirstName;
        this.LastName = LastName;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }

    public override string ToString() => $"{FirstName},{LastName}";

    public UserFullName(string value)
    {
        var values = value.Split(",");

        FirstName = values.First();
        LastName = values.Last();
    }
}