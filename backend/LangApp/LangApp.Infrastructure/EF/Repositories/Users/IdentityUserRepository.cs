using LangApp.Application.Auth.Exceptions;
using LangApp.Core.Entities.Users;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LangApp.Infrastructure.EF.Repositories.Users;

internal sealed class IdentityUserRepository : IApplicationUserRepository
{
    private readonly UserManager<IdentityApplicationUser> _userManager;
    private readonly IApplicationUserFactory _factory;
    private readonly DbSet<IdentityApplicationUser> _users;

    public IdentityUserRepository(
        UserManager<IdentityApplicationUser> userManager,
        IApplicationUserFactory factory,
        WriteDbContext context)
    {
        _userManager = userManager;
        _factory = factory;
        _users = context.Users;
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

    public async Task AddAsync(ApplicationUser user, string password)
    {
        var identityUser = user.ToIdentityModel();

        var result = await _userManager.CreateAsync(identityUser, password);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e => e.Description));
        }
    }

    public async Task UpdateAsync(ApplicationUser model)
    {
        var user = await _userManager.FindByIdAsync(model.Id.ToString());

        if (user is null)
        {
            throw new UserNotFoundException(model.Id);
        }

        var identityModel = model.ToIdentityModel();
        user.UserName = identityModel.UserName;
        user.FullName = identityModel.FullName;
        user.PictureUrl = identityModel.PictureUrl;

        await _userManager.UpdateAsync(user);
    }

    public Task DeleteAsync(ApplicationUser user)
    {
        throw new NotImplementedException();
    }

    public async Task<ApplicationUser?> GetByUsername(string username)
    {
        return (await _userManager.FindByNameAsync(username))?.ToDomainModel(_factory);
    }
}
