using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;

namespace LangApp.Application.Posts.Queries;

public record GetPostsByGroup(
    Guid GroupId,
    Guid UserId
) : PagedQuery<PagedResult<PostSlimDto>>;