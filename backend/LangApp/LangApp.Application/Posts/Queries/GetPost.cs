using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;

namespace LangApp.Application.Posts.Queries;

public class GetPost : IQuery<PostDto>
{
    public Guid Id { get; set; }
}