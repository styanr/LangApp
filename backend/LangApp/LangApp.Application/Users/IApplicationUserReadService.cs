namespace LangApp.Application.Users;

public interface IApplicationUserReadService
{
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}