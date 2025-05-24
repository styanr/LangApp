using LangApp.Application.Submissions.Dto;

namespace LangApp.Api.Endpoints.Submissions.Models;

public record CreateAssignmentSubmissionRequest(
    List<CreateActivitySubmissionDto> ActivitySubmissionDtos
);