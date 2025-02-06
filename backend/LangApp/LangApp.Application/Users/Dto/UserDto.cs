using LangApp.Core.Enums;

namespace LangApp.Application.Users.Dto;

public record UserDto(
    Guid Id,
    string Username,
    FullNameDto FullName,
    string? PictureUrl,
    AppUserRole Role
);