namespace LangApp.Core.Exceptions.Assignments;

public class AssignmentDueTimeInvalid(DateTime dueTime)
    : LangAppException($"The following date is invalid for this assignment: {dueTime}");