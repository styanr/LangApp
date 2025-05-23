using LangApp.Core.Enums;

namespace LangApp.Application.Submissions.Dto;

public record AssignmentSubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    string StudentName,
    string? StudentPictureUrl,
    DateTime SubmittedAt,
    GradeStatus Status,
    double Score,
    List<ActivitySubmissionDto> ActivitySubmissions
);