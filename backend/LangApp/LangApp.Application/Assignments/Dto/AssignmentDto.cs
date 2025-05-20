namespace LangApp.Application.Assignments.Dto;

public record AssignmentDto(
    Guid Id,
    string Name,
    string? Description,
    Guid AuthorId,
    Guid StudyGroupId,
    DateTime DueTime,
    int MaxScore,
    bool Submitted,
    List<ActivityDto> Activities
);

public record AssignmentSlimDto(
    Guid Id,
    string Name,
    string? Description,
    Guid AuthorId,
    Guid StudyGroupId,
    DateTime DueTime,
    int MaxScore,
    bool Submitted,
    int ActivityCount
);