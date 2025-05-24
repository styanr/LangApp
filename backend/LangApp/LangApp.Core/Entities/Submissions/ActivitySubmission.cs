using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Events.Submissions;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Submissions;

namespace LangApp.Core.Entities.Submissions;

public class ActivitySubmission : BaseEntity
{
    public Guid ActivityId { get; private set; }
    public ActivityType Type { get; private set; }

    public SubmissionDetails Details { get; private set; }

    public GradeStatus Status { get; private set; } = GradeStatus.Pending;
    public SubmissionGrade? Grade { get; private set; }
    public string? FailureReason { get; private set; }

    private ActivitySubmission()
    {
    }

    protected ActivitySubmission(Guid activityId, SubmissionDetails details, ActivityType type)
    {
        ActivityId = activityId;
        Details = details;
        Type = type;
    }

    internal ActivitySubmission(Guid activityId, SubmissionDetails details, ActivityType type,
        Guid id) :
        base(id)
    {
        ActivityId = activityId;
        Details = details;
        Type = type;
    }

    internal static ActivitySubmission Create(
        Guid activityId,
        SubmissionDetails details, ActivityType type, Guid id)
    {
        var submission = new ActivitySubmission(activityId, details, type, id);
        if (submission.Type == ActivityType.Writing)
        {
            submission.Status = GradeStatus.NeedsReview;
        }

        return submission;
    }

    public void UpdateGrade(SubmissionGrade submissionGradeResult)
    {
        Grade = submissionGradeResult;
        Status = GradeStatus.Completed;
    }

    public void Fail(string? reason)
    {
        FailureReason = reason;
        Status = GradeStatus.Failed;
    }
}
