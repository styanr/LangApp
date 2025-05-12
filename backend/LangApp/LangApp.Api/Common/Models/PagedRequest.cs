namespace LangApp.Api.Common.Models;

public record PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
