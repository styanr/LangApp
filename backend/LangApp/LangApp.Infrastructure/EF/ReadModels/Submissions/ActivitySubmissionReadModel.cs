using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Infrastructure.EF.Models.Submissions;

public class ActivitySubmissionReadModel
{
    public Guid Id { get; set; }
    public Guid ActivityId { get; set; }
    public Guid AssignmentSubmissionId { get; set; }
    public ActivityType Type { get; set; }
    public GradeStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public SubmissionGradeReadModel? Grade { get; set; }
    public SubmissionDetailsReadModel Details { get; set; }
}
