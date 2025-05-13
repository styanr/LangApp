using LangApp.Core.Common.Exceptions;

namespace LangApp.Application.Assignments.Exceptions;

public class AssignmentNotFound : NotFoundException
{
    public Guid AssignmentId { get; }

    public AssignmentNotFound(Guid assignmentId) : base($"Assignment with id {assignmentId} not found.")
    {
        AssignmentId = assignmentId;
    }
}

public class ActivityNotFound : NotFoundException
{
    public Guid ActivityId { get; }

    public ActivityNotFound(Guid activityId) : base($"Activity with id {activityId} not found.")
    {
        ActivityId = activityId;
    }
}
