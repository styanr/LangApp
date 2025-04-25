using LangApp.Core.Enums;

namespace LangApp.Infrastructure.EF.Models.Submissions;

public class SubmissionReadModel
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public AssignmentType Type { get; set; }
    public SubmissionDetailsReadModel Details { get; set; }
    public DateTime SubmittedAt { get; set; }
    public GradeStatus Status { get; set; }
    public SubmissionGradeReadModel? Grade { get; set; }
}