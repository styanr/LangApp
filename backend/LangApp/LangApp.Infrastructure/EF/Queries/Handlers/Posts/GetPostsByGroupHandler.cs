using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Posts;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Posts;

internal sealed class GetPostsByGroupHandler : IQueryHandler<GetPostsByGroup, PagedResult<PostSlimDto>>
{
    private readonly DbSet<PostReadModel> _posts;

    public GetPostsByGroupHandler(ReadDbContext context)
    {
        _posts = context.Posts;
    }

    public async Task<PagedResult<PostSlimDto>?> HandleAsync(GetPostsByGroup query)
    {
        const int contentPreviewLength = 15;
        var totalCount = await _posts.Where(p => p.GroupId == query.GroupId).CountAsync();

        var posts = await _posts
            .Where(p => p.GroupId == query.GroupId)
            .TakePage(query.PageNumber, query.PageSize)
            .AsNoTracking()
            .Select(p => new PostSlimDto(
                p.Id,
                p.AuthorId,
                p.Type,
                p.Title,
                p.Content.Substring(0, contentPreviewLength)))
            .ToListAsync();

        return new PagedResult<PostSlimDto>(
            posts,
            totalCount,
            query.PageNumber,
            query.PageSize
        );
    }
}