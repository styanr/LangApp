namespace LangApp.Application.Common.Queries.Abstractions;

public abstract class PagedQuery<TResult> : IQuery<TResult>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}