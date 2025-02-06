using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;

namespace LangApp.Application.Users.Queries;

public class SearchUsers : PagedQuery<PagedResult<UserDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
}