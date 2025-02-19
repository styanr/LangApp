namespace LangApp.Application.Common.Queries.Abstractions;

public interface IQueryDispatcher
{
    Task<TResult?> QueryAsync<TResult>(IQuery<TResult> query);
}