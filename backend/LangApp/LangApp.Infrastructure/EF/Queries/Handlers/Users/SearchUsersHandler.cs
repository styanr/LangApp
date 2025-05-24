using LangApp.Application.Common;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Users;

internal sealed class SearchUsersHandler : IQueryHandler<SearchUsers, PagedResult<UserDto>>
{
    private readonly DbSet<UserReadModel> _users;

    public SearchUsersHandler(ReadDbContext context)
    {
        _users = context.Users;
    }

    public async Task<PagedResult<UserDto>?> HandleAsync(SearchUsers query)
    {
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            return new(
                [],
                0,
                query.PageNumber,
                query.PageSize
            );
        }

        var users = await _users
            .Where(u =>
                Microsoft.EntityFrameworkCore.EF.Functions.ILike(u.Username, $"%{query.SearchTerm}%"))
            .OrderBy(u => u.Username)
            .Where(u => u.Id != query.UserId)
            .TakePage(query.PageNumber, query.PageSize)
            .Select(u => u.ToDto())
            .AsNoTracking()
            .ToListAsync();

        var count = await _users
            .Where(u =>
                Microsoft.EntityFrameworkCore.EF.Functions.ILike(u.Username, $"%{query.SearchTerm}%"))
            .CountAsync();

        return new(
            users,
            count,
            query.PageNumber,
            query.PageSize
        );
    }
}
