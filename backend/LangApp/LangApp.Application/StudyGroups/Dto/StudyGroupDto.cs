namespace LangApp.Application.StudyGroups.Dto;

public record StudyGroupDto(
    Guid Id,
    string Name,
    string Language,
    StudyGroupOwnerDto Owner,
    IEnumerable<StudyGroupMemberDto> Members);