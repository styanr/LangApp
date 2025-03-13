using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;

namespace LangApp.Application.Users.Queries;

public record SearchUsers(
    string SearchTerm
) : PagedQuery<PagedResult<UserDto>>;