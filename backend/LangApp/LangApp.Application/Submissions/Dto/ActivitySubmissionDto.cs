using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record CreateActivitySubmissionDto(
    Guid ActivityId,
    ActivitySubmissionDetailsDto Details
);

public record ActivitySubmissionDto(
    Guid Id,
    Guid ActivityId,
    ActivityType Type,
    ActivitySubmissionDetailsDto Details,
    GradeStatus Status,
    SubmissionGradeDto? Grade
);

public class SubmissionGradeDto
{
    public double ScorePercentage { get; set; }
    public string Feedback { get; set; }
}
