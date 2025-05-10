using LangApp.Core.Enums;

namespace LangApp.Infrastructure.EF.Models.Submissions;

public class AssignmentSubmissionReadModel
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public GradeStatus Status { get; set; }
    public int Score { get; set; }

    public List<ActivitySubmissionReadModel> ActivitySubmissions { get; set; }
}
