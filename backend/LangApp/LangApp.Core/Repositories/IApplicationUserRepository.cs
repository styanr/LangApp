using LangApp.Core.Entities;
using LangApp.Core.Entities.Users;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetAsync(Guid id);
    Task AddAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
}