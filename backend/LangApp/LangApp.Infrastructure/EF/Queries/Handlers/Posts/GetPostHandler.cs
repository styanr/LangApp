using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Services.Policies.Posts;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Posts;

internal sealed class GetPostHandler : IQueryHandler<GetPost, PostDto>
{
    private readonly DbSet<PostReadModel> _posts;
    private readonly IPostAccessPolicyService _policy;

    public GetPostHandler(ReadDbContext context, IPostAccessPolicyService policy)
    {
        _policy = policy;
        _posts = context.Posts;
    }

    public async Task<PostDto?> HandleAsync(GetPost query)
    {
        var post = await _posts
            .Where(p => p.Id == query.Id && !p.Archived)
            .Select(p => new PostDto
            (
                p.Id,
                p.Type,
                p.AuthorId,
                p.GroupId,
                p.Title,
                p.Content,
                p.CreatedAt,
                p.IsEdited,
                p.Media ?? new List<string>()
            )).AsNoTracking().SingleOrDefaultAsync();
        if (post is null)
        {
            return null;
        }

        var isAllowed = await _policy.IsSatisfiedBy(post.Id, post.GroupId, query.UserId);

        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId);
        }

        return post;
    }
}