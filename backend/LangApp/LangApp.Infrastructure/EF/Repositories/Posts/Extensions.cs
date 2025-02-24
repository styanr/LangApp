using LangApp.Core.Entities.Posts;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Posts;

public static class Extensions
{
    public static IQueryable<Post> WhereShowArchived(this IQueryable<Post> query, bool showArchived)
    {
        return !showArchived ? query.Where(p => !p.Archived) : query;
    }
}