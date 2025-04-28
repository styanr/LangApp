using LangApp.Application.Assignments.Dto.Pronunciation;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record CreatePronunciationAssignmentRequest(
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    PronunciationAssignmentDetailsDto Details
);
