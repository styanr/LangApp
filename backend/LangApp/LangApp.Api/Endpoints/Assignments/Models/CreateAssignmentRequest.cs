using LangApp.Application.Assignments.Dto;

namespace LangApp.Api.Endpoints.Assignments.Models;

public record CreateAssignmentRequest(
    string Name,
    Guid GroupId,
    DateTime DueDate,
    List<ActivityDto> Activities
);
