using LangApp.Core.Enums;

namespace LangApp.Api.Endpoints.Posts.Models;

public record CreatePostRequest(
    Guid GroupId,
    PostType Type,
    string Title,
    string Content,
    List<string>? Media = null
);