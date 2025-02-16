namespace LangApp.Infrastructure.EF.Queries.Handlers;

public static class QueryExtensions
{
    public static IQueryable<T> TakePage<T>(this IQueryable<T> query, int pageNumber, int pageSize) where T : class
    {
        var skipAmount = (pageNumber - 1) * pageSize;

        return query.Skip(skipAmount).Take(pageSize);
    }
}