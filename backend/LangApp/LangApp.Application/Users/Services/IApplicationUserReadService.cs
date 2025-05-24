using LangApp.Core.Enums;

namespace LangApp.Application.Users.Services;

public interface IApplicationUserReadService
{
    Task<bool> Exists(Guid id);
    Task<UserRole> GetRoleAsync(Guid id);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}
