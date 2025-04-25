using LangApp.Core.Enums;

namespace LangApp.Application.Assignments.Dto;

public record AssignmentDto(
    Guid Id,
    Guid AuthorId,
    Guid GroupId,
    DateTime DueTime,
    int MaxScore,
    AssignmentType Type,
    AssignmentDetailsDto Details
);