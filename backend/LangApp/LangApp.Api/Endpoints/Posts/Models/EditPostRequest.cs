namespace LangApp.Api.Endpoints.Posts.Models;

public record EditPostRequest(string Content, List<string>? Media = null);
