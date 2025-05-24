namespace LangApp.Application.Posts.Dto;

public record PostCommentDto(
    Guid Id,
    Guid AuthorId,
    string AuthorName,
    string? AuthorProfilePicture,
    string Content,
    DateTime CreatedAt,
    DateTime? EditedAt);