namespace LangApp.Application.Submissions.Dto;

public record AssignmentSubmissionDto(
    Guid Id,
    Guid AssignmentId,
    Guid StudentId,
    List<ActivitySubmissionDto> ActivitySubmissions
);
