using LangApp.Application.Submissions.Dto;

namespace LangApp.Application.Assignments.Dto;

public record AssignmentSubmissionsStatisticsDto(
    Guid AssignmentId,
    int SubmissionCount,
    int PendingCount,
    int FailedCount,
    int CompletedCount,
    int NeedsReviewCount,
    List<AssignmentSubmissionInfoDto> Submissions
);

public record AssignmentSubmissionInfoDto(
    Guid SubmissionId,
    Guid StudentId,
    string StudentName,
    string? StudentPictureUrl
);