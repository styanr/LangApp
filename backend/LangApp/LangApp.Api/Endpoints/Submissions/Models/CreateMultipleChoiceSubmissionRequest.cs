using LangApp.Application.Submissions.Dto;
using LangApp.Core.Enums;

namespace LangApp.Api.Endpoints.Submissions.Models;

public record CreateMultipleChoiceSubmissionRequest(
    Guid AssignmentId,
    AssignmentType Type,
    MultipleChoiceSubmissionDetailsDto Details
);