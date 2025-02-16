using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;

namespace LangApp.Application.Posts.Queries;

public class GetPostsByGroup : PagedQuery<PagedResult<PostSlimDto>>
{
    public Guid GroupId { get; set; }
}