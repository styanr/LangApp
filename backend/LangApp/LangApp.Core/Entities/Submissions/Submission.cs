using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Core.Entities.Submissions;

public class Submission : AggregateRoot
{
    public Guid AssignmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public AssignmentType Type { get; private set; }
    public SubmissionDetails Details { get; private set; }
    public DateTime SubmittedAt { get; private set; } = DateTime.UtcNow;
    public GradeStatus Status { get; private set; } = GradeStatus.Pending;
    public SubmissionGrade? Grade { get; private set; }

    protected Submission(SubmissionDetails details, AssignmentType type)
    {
        Details = details;
        Type = type;
    }

    internal Submission(Guid assignmentId, Guid studentId, SubmissionDetails details, AssignmentType type, Guid id) :
        base(id)
    {
        AssignmentId = assignmentId;
        StudentId = studentId;
        Details = details;
        Type = type;
    }

    public void UpdateGrade(SubmissionGrade submissionGradeResult)
    {
        Grade = submissionGradeResult;
    }
}