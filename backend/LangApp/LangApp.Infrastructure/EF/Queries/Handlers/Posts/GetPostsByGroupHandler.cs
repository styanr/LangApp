using System.Text;
using LangApp.Application.Common;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Posts;

internal sealed class GetPostsByGroupHandler : IQueryHandler<GetPostsByGroup, PagedResult<PostSlimDto>>
{
    private readonly DbSet<PostReadModel> _posts;
    private readonly DbSet<StudyGroupReadModel> _groups;

    public GetPostsByGroupHandler(ReadDbContext context)
    {
        _posts = context.Posts;
        _groups = context.StudyGroups;
    }

    public async Task<PagedResult<PostSlimDto>?> HandleAsync(GetPostsByGroup query)
    {
        var group = await _groups
            .Include(g => g.Members)
            .AsNoTracking()
            .SingleOrDefaultAsync(g => g.Id == query.GroupId);

        if (group is null)
        {
            return null;
        }

        // Direct permission check
        bool isAllowed = false;

        // Group owner can access all posts in their group
        if (group.OwnerId == query.UserId)
        {
            isAllowed = true;
        }
        // Group members can access posts in the group
        else if (group.Members.Any(m => m.Id == query.UserId))
        {
            isAllowed = true;
        }

        if (!isAllowed)
        {
            throw new UnauthorizedException(query.UserId);
        }

        const int contentPreviewLength = 30;
        var totalCount = await _posts.Where(p => p.GroupId == query.GroupId).CountAsync();

        var posts = await _posts
            .Where(p => p.GroupId == query.GroupId && !p.Archived)
            .Include(p => p.Author)
            .OrderByDescending(p => p.CreatedAt)
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(p => new PostSlimDto(
                p.Id,
                p.AuthorId,
                p.Author.Username,
                p.Type,
                p.Title,
                ToPreview(p.Content, contentPreviewLength),
                p.CreatedAt,
                p.IsEdited,
                p.Media.Count)
            )
            .ToListAsync();

        return new PagedResult<PostSlimDto>(
            posts,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }

    private static string ToPreview(string value, int previewLength)
    {
        var builder = new StringBuilder(value.Substring(0, Math.Min(previewLength, value.Length)));
        if (builder.Length < value.Length)
        {
            builder.Append("...");
        }

        return builder.ToString();
    }
}