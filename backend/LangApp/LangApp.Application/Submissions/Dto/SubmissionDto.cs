using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    AssignmentType Type,
    SubmissionDetailsDto Details,
    DateTime SubmittedAt,
    GradeStatus Status,
    SubmissionGradeDto? Grade
);

public class SubmissionGradeDto
{
    public double ScorePercentage { get; set; }
    public string Feedback { get; set; }
}
