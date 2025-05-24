using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Models.Assignments;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Models.Submissions;

public class AssignmentSubmissionReadModel
{
    public Guid Id { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public GradeStatus Status { get; set; }
    public double Score { get; set; }

    public List<ActivitySubmissionReadModel> ActivitySubmissions { get; set; }
    public UserReadModel Student { get; set; }
    public AssignmentReadModel Assignment { get; set; }
}