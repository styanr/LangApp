using LangApp.Core.Enums;

namespace LangApp.Application.Posts.Dto;

public record PostDto(
    Guid Id,
    PostType Type,
    Guid AuthorId,
    Guid GroupId,
    string Title,
    string Content,
    DateTime EditedAt,
    IEnumerable<string> Media);