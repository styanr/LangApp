using LangApp.Core.Enums;

namespace LangApp.Application.Posts.Dto;

public record PostSlimDto(Guid Id, Guid AuthorId, PostType Type, string Title, string ContentPreview);