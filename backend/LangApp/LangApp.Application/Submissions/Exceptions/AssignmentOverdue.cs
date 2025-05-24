using LangApp.Core.Exceptions;

namespace LangApp.Application.Submissions.Commands;

public class AssignmentOverdue : LangAppException
{
    public Guid AssignmentId { get; }
    public DateTime AssignmentDueDate { get; }

    public AssignmentOverdue(Guid assignmentId, DateTime assignmentDueDate) : base(
        $"Cannot create submission for overdue assignment with ID={assignmentId}. Due date={assignmentDueDate}.")
    {
        AssignmentId = assignmentId;
        AssignmentDueDate = assignmentDueDate;
    }
}
