using LangApp.Application.Users.Services;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Services;

internal sealed class ApplicationUserReadService : IApplicationUserReadService
{
    private readonly DbSet<UserReadModel> _users;

    public ApplicationUserReadService(ReadDbContext context)
    {
        _users = context.Users;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _users.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _users.AnyAsync(u => u.Email == email);
    }
}