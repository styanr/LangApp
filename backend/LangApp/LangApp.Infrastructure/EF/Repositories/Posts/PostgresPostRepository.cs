using LangApp.Core.Entities.Posts;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Posts;

internal sealed class PostgresPostRepository : IPostRepository
{
    private readonly DbSet<Post> _posts;
    private readonly WriteDbContext _context;

    public PostgresPostRepository(WriteDbContext context)
    {
        _context = context;
        _posts = context.Posts;
    }

    public Task<Post?> GetAsync(Guid id)
    {
        return GetAsync(id, false);
    }

    public Task<Post?> GetAsync(Guid id, bool showArchived)
    {
        return _posts
            .WhereShowArchived(showArchived)
            .Include(p => p.Comments)
            .SingleOrDefaultAsync(p => p.Id == id);
    }


    public async Task AddAsync(Post post)
    {
        await _posts.AddAsync(post);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Post post)
    {
        _posts.Update(post);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Post post)
    {
        _posts.Remove(post);

        await _context.SaveChangesAsync();
    }
}
