namespace LangApp.Application.Common.Queries.Abstractions;

public interface IQueryHandler<TQuery, TResult> where TQuery : class, IQuery<TResult>
{
    Task<TResult?> HandleAsync(TQuery query);
}
