namespace LangApp.Core.Exceptions.Assignments;

public class InvalidAssignmentNameException(string name)
    : LangAppException(string.IsNullOrWhiteSpace(name) ? "Assignment name cannot be null or empty." : $"Assignment name \"{name}\" is too long. It cannot be longer than 100 characters.")
{
    public string Name { get; } = name;
}
