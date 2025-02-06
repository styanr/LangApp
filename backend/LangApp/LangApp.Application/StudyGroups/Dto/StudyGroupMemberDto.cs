using LangApp.Application.Users.Dto;

namespace LangApp.Application.StudyGroups.Dto;

public record StudyGroupMemberDto(Guid Id, FullNameDto FullName, string? PictureUrl);