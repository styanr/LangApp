using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Exceptions;
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
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetPostHandler(ReadDbContext context, IPostAccessPolicyService policy)
    {
        _posts = context.Posts;
        _groups = context.StudyGroups;
    }

    public async Task<PostDto?> HandleAsync(GetPost query)
    {
        // First, check if post exists and get minimal information needed for permission check
        var postInfo = await _posts
            .Where(p => p.Id == query.Id && (!p.Archived || p.AuthorId == query.UserId))
            .Select(p => new { p.Id, p.GroupId, p.AuthorId })
            .AsNoTracking()
            .SingleOrDefaultAsync();

        if (postInfo is null)
        {
            return null;
        }

        var access = await _groups
            .Where(g => g.Id == postInfo.GroupId)
            .Select(g => new
            {
                IsOwner = g.OwnerId == query.UserId,
                IsMember = g.Members.Any(m => m.Id == query.UserId)
            })
            .SingleOrDefaultAsync();

        if (access == null || (!access.IsOwner && !access.IsMember))
        {
            throw new UnauthorizedException(query.UserId);
        }

        var post = await _posts
            .Include(p => p.Author)
            .Include(p => p.Group)
            .Include(p => p.Comments.OrderByDescending(c => c.CreatedAt))
            .ThenInclude(c => c.Author)
            .Where(p => p.Id == query.Id)
            .AsNoTracking()
            .AsSplitQuery()
            .SingleOrDefaultAsync();

        if (post is null)
        {
            throw new PostNotFoundException(query.Id);
        }

        return new PostDto(
            post.Id,
            post.Type,
            post.AuthorId,
            post.Author.Username,
            post.Author.PictureUrl,
            post.GroupId,
            post.Title,
            post.Content,
            post.CreatedAt,
            post.IsEdited,
            post.Comments.Select(c => new PostCommentDto(
                    c.Id,
                    c.AuthorId,
                    c.Author.Username,
                    c.Author.PictureUrl,
                    c.Content,
                    c.CreatedAt,
                    c.EditedAt))
                .ToList(),
            post.Media
        );
    }
}