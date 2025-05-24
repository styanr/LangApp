using LangApp.Application.Users.Services;
using LangApp.Core.Enums;
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

    public Task<bool> Exists(Guid id)
    {
        return _users.AnyAsync(u => u.Id == id);
    }

    public Task<UserRole> GetRoleAsync(Guid id)
    {
        return _users.Where(u => u.Id == id).Select(u => u.Role).FirstOrDefaultAsync();
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        return _users.AnyAsync(u => u.Username == username);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        return _users.AnyAsync(u => u.Email == email);
    }
}
