using LangApp.Application.Assignments.Dto;

namespace LangApp.Application.Submissions.Dto;

public record UserGroupSubmissionsDto(
    Guid GroupId,
    Guid UserId,
    List<UserGroupSubmissionDto> Submissions
);

public record UserGroupSubmissionDto(AssignmentDto Assignment, AssignmentSubmissionDto Submission);