using LangApp.Application.Assignments.Dto;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record CreateAssignmentRequest(
    string Name,
    string? Description,
    Guid GroupId,
    DateTime DueDate,
    List<CreateActivityDto> Activities
);