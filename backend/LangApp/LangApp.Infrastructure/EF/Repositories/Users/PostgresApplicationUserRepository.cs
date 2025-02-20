using LangApp.Core.Entities.Users;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Users;

internal sealed class PostgresApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly IApplicationUserFactory _factory;

    public PostgresApplicationUserRepository(UserManager<IdentityApplicationUser> userManager,
        IApplicationUserFactory factory)
    {
        _userManager = userManager;
        _factory = factory;
    }

    public async Task<ApplicationUser?> GetAsync(Guid id)
    {
        var identityUser = await _userManager.FindByIdAsync(id.ToString());

        return identityUser?.ToDomainModel(_factory);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAsync(IEnumerable<Guid> ids)
    {
        return await _userManager.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => u.ToDomainModel(_factory))
            .ToListAsync();
    }

    public Task AddAsync(ApplicationUser user)
    {
        var identityUser = user.ToIdentityModel();

        return _userManager.CreateAsync(identityUser);
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