using System.Text;
using LangApp.Application.Common;
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
        const int contentPreviewLength = 50;
        var totalCount = await _posts.Where(p => p.GroupId == query.GroupId).CountAsync();

        var group = await _groups
            .Where(g => g.Id == query.GroupId)
            .Include(g => g.Members)
            .SingleOrDefaultAsync() ?? throw new StudyGroupNotFoundException(query.GroupId);

        if (group.Members.All(m => m.Id != query.UserId) && group.OwnerId != query.UserId)
        {
            throw new UnauthorizedException(query.UserId, group);
        }

        var posts = await _posts
            .Where(p => p.GroupId == query.GroupId && !p.Archived)
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(p => new PostSlimDto(
                p.Id,
                p.AuthorId,
                p.Type,
                p.Title,
                ToPreview(p.Content, 30),
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
        var builder = new StringBuilder(value.Substring(0, previewLength));
        if (builder.Length != value.Length)
        {
            builder.Append("...");
        }

        return builder.ToString();
    }
}