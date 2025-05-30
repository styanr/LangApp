namespace LangApp.Core.Exceptions.Assignments;

public class InvalidAssignmentDescriptionException(string? description)
    : LangAppException($"Assignment description is too long. It cannot be longer than 500 characters. Submitted description: \"{description}\"")
{
    public string? Description { get; } = description;
}
