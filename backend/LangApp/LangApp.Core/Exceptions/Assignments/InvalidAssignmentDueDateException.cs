namespace LangApp.Core.Exceptions.Assignments;

public class InvalidAssignmentDueDateException(DateTime dueDate)
    : LangAppException($"Due date \"{dueDate}\" is invalid. It cannot be in the past.")
{
    public DateTime DueDate { get; } = dueDate;
}
