using LangApp.Application.Assignments.Dto.FillInTheBlank;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record AddFillInTheBlankAssignmentRequest(
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    FillInTheBlankAssignmentDetailsDto Details
);