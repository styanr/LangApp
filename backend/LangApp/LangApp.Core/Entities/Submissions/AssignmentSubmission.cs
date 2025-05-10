using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Events.Submissions;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Submissions;

public class AssignmentSubmission : AggregateRoot
{
    public Guid AssignmentId { get; private set; }
    public Guid StudentId { get; private set; }
    public DateTime SubmittedAt { get; private set; } = DateTime.UtcNow;
    public GradeStatus Status { get; private set; } = GradeStatus.Pending;
    public double Score { get; private set; }

    private readonly List<ActivitySubmission> _activitySubmissions = [];

    public IReadOnlyList<ActivitySubmission> ActivitySubmissions => _activitySubmissions;

    private AssignmentSubmission()
    {
    }

    internal AssignmentSubmission(Guid assignmentId, Guid studentId, int score, Guid id) : base(id)
    {
        AssignmentId = assignmentId;
        StudentId = studentId;
        Score = score;
    }

    internal AssignmentSubmission(Guid assignmentId, Guid studentId, int score, Guid id,
        List<ActivitySubmission> activitySubmissions) : this(assignmentId, studentId, score, id)
    {
        _activitySubmissions.AddRange(activitySubmissions);
    }

    internal static AssignmentSubmission Create(Guid assignmentId, Guid studentId, int score, Guid id,
        List<ActivitySubmission> activitySubmissions)
    {
        var submission = new AssignmentSubmission(assignmentId, studentId, score, id, activitySubmissions);

        submission.AddEvent(new SubmissionCreated(submission));

        return submission;
    }

    public void AddActivitySubmission(ActivitySubmission activitySubmission)
    {
        _activitySubmissions.Add(activitySubmission);
    }

    public void RemoveActivitySubmission(ActivitySubmission activitySubmission)
    {
        _activitySubmissions.Remove(activitySubmission);
    }

    public void GradeActivitySubmission(ActivitySubmission activitySubmission, SubmissionGrade submissionGrade)
    {
        var index = _activitySubmissions.IndexOf(activitySubmission);
        _activitySubmissions[index].UpdateGrade(submissionGrade);

        // if (_activitySubmissions.All(a => a.Status == GradeStatus.Completed))
        // {
        //     Status = GradeStatus.Completed;
        // }
        //
        // if (_activitySubmissions.Any(a => a.Status == GradeStatus.Failed))
        // {
        //     Status = GradeStatus.Failed;
        // }
    }

    public void UpdateScore(double score)
    {
        Score = score;
    }

    public void Fail()
    {
        Status = GradeStatus.Failed;
    }

    public void FailActivitySubmission(ActivitySubmission activitySubmission)
    {
        activitySubmission.Fail();
        Fail();
    }
}
