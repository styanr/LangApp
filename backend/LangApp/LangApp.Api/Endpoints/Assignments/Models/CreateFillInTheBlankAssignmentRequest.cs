using LangApp.Application.Assignments.Dto.FillInTheBlank;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record CreateFillInTheBlankAssignmentRequest(
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    FillInTheBlankActivityDetailsDto Details
);