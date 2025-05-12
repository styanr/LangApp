using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record AssignmentSubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    DateTime SubmittedAt,
    GradeStatus Status,
    double Score,
    List<ActivitySubmissionDto> ActivitySubmissions
);
