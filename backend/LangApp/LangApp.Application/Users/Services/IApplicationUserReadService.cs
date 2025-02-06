namespace LangApp.Application.Users.Services;

public interface IApplicationUserReadService
{
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
}