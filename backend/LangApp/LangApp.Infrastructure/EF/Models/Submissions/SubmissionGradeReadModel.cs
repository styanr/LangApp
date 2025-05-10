namespace LangApp.Infrastructure.EF.Models.Submissions;

public class SubmissionGradeReadModel
{
    public Guid SubmissionId { get; set; }
    public double ScorePercentage { get; set; }
    public string Feedback { get; set; }
}
