using LangApp.Core.Enums;

namespace LangApp.Application.Posts.Dto;

public record PostSlimDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string? AuthorProfilePicture,
    UserRole AuthorRole,
    PostType Type,
    string Title,
    string ContentPreview,
    DateTime CreatedAt,
    bool IsEdited,
    int MediaCount);