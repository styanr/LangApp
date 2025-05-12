namespace LangApp.Application.Assignments.Dto;

public record AssignmentDto(
    Guid Id,
    string Name,
    Guid AuthorId,
    Guid StudyGroupId,
    DateTime DueTime,
    List<ActivityDto> Activities
);
