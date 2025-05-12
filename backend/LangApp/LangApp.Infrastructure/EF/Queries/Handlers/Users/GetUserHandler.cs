using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Application.Users.Dto;
using LangApp.Application.Users.Queries;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Queries.Handlers.Users;

internal sealed class GetUserHandler : IQueryHandler<GetUser, UserDto>
{
    private readonly DbSet<UserReadModel> _users;

    public GetUserHandler(ReadDbContext context)
    {
        _users = context.Users;
    }

    public Task<UserDto?> HandleAsync(GetUser query)
    {
        return _users.Where(u => u.Id == query.Id)
            .Select(u => u.ToDto())
            .AsNoTracking().SingleOrDefaultAsync();
    }
}
