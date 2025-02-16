namespace LangApp.Application.StudyGroups.Dto;

public record StudyGroupSlimDto(
    Guid Id,
    string Name,
    string Language
);