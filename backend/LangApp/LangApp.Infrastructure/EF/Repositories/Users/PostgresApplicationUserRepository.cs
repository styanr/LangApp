using LangApp.Core.Entities.Users;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Users;

internal sealed class PostgresApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserManager<IdentityApplicationUser> _userManager;

    public PostgresApplicationUserRepository(UserManager<IdentityApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public Task<ApplicationUser?> GetAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }
}