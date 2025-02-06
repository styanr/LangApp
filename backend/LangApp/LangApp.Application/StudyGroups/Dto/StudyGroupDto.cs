namespace LangApp.Application.StudyGroups.Dto;

public record StudyGroupDto(
    string Name,
    string Language,
    StudyGroupOwnerDto Owner,
    IEnumerable<StudyGroupMemberDto> Members);