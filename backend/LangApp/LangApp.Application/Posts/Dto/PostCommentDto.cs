namespace LangApp.Application.Posts.Dto;

public record PostCommentDto(
    Guid Id,
    Guid AuthorId,
    string Content,
    DateTime CreatedAt,
    DateTime? EditedAt);
