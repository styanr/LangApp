using LangApp.Core.Exceptions;

namespace LangApp.Application.Submissions.Exceptions;

public class SubmissionAlreadyExists : LangAppException
{
    public Guid AssignmentId { get; }
    public Guid UserId { get; }

    public SubmissionAlreadyExists(Guid assignmentId, Guid userId) : base(
        $"Submission for assignment {assignmentId} already exists for user {userId}")
    {
        AssignmentId = assignmentId;
        UserId = userId;
    }
}
