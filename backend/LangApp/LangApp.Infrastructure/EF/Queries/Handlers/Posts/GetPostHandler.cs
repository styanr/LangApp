using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Posts;

internal sealed class GetPostHandler : IQueryHandler<GetPost, PostDto>
{
    private readonly DbSet<PostReadModel> _posts;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetPostHandler(ReadDbContext context)
    {
        _posts = context.Posts;
        _groups = context.StudyGroups;
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
                p.EditedAt,
                p.Media ?? new List<string>()
            )).AsNoTracking().SingleOrDefaultAsync();
        if (post is null)
        {
            return null;
        }

        var group = await _groups
            .Where(g => g.Id == post.GroupId)
            .Include(g => g.Members)
            .SingleOrDefaultAsync() ?? throw new StudyGroupNotFoundException(post.GroupId);

        if (group.Members.All(m => m.Id != query.UserId) && group.OwnerId != query.UserId)
        {
            throw new UnauthorizedException(query.UserId, group);
        }

        return post;
    }
}