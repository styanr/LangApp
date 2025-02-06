using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;

namespace LangApp.Application.Posts.Queries;

public class GetPostsByGroup : PagedQuery<PostSlimDto>
{
    public Guid GroupId { get; set; }
}