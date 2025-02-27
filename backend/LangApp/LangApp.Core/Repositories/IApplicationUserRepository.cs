using LangApp.Core.Entities;
using LangApp.Core.Entities.Users;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Repositories;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetAsync(Guid id);
    Task<IEnumerable<ApplicationUser>> GetAsync(IEnumerable<Guid> ids);
    Task AddAsync(ApplicationUser user, string password);
    Task UpdateAsync(ApplicationUser user);
    Task DeleteAsync(ApplicationUser user);
    Task<ApplicationUser?> GetByUsername(string username);
}