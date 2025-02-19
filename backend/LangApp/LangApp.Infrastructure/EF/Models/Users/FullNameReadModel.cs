namespace LangApp.Infrastructure.EF.Models.Users;

public class FullNameReadModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public override string ToString() => $"{FirstName},{LastName}";

    private FullNameReadModel()
    {
    }

    public FullNameReadModel(string value)
    {
        var values = value.Split(",");

        FirstName = values.First();
        LastName = values.Last();
    }
}