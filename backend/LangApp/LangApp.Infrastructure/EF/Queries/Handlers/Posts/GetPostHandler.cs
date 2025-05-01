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
            .Include(p => p.Group)
            .ThenInclude(g => g.Members)
            .Where(p => p.Id == query.Id &&
                        (!p.Archived ||
                         p.AuthorId == query.UserId)) // return archived posts only if the user is the author
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (post is null)
        {
            return null;
        }

        // TODO: Access checking in the query handler is not ideal, but I don't see any better
        // and faster (performance-wise) way to do it.

        // Direct permission check
        var isAllowed = false;

        // Group owner can access all posts in their group
        if (post.Group.OwnerId == query.UserId)
        {
            isAllowed = true;
        }
        // Post author can access their own post
        else if (post.AuthorId == query.UserId)
        {
            isAllowed = true;
        }
        // Group members can access posts in the group
        else if (post.Group.Members.Any(m => m.Id == query.UserId))
        {
            isAllowed = true;
        }

        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId);
        }

        // Map to DTO after access check
        return new PostDto(
            post.Id,
            post.Type,
            post.AuthorId,
            post.GroupId,
            post.Title,
            post.Content,
            post.CreatedAt,
            post.IsEdited,
            post.Media
        );
    }
}
