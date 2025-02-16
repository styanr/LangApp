using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Posts.Dto;
using LangApp.Application.Posts.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Posts;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Posts;

internal sealed class GetPostHandler : IQueryHandler<GetPost, PostDto>
{
    private readonly DbSet<PostReadModel> _posts;

    public GetPostHandler(ReadDbContext context)
    {
        _posts = context.Posts;
    }

    public Task<PostDto?> HandleAsync(GetPost query)
    {
        return _posts
            .Include(p => p.Media)
            .Where(p => p.Id == query.Id)
            .Select(p => new PostDto
            (
                p.Id,
                p.Type,
                p.AuthorId,
                p.GroupId,
                p.Title,
                p.Content,
                p.EditedAt,
                p.Media
            )).AsNoTracking().SingleOrDefaultAsync();
    }
}