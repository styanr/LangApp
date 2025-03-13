using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;

namespace LangApp.Application.Posts.Queries;

public record GetPost(
    Guid Id,
    Guid UserId
) : IQuery<PostDto>;