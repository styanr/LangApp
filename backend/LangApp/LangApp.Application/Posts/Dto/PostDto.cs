using LangApp.Core.Enums;

namespace LangApp.Application.Posts.Dto;

public record PostDto(
    Guid Id,
    PostType Type,
    Guid AuthorId,
    Guid GroupId,
    string Title,
    string Content,
    DateTime CreatedAt,
    bool IsEdited,
    List<PostCommentDto> Comments,
    IEnumerable<string> Media);
