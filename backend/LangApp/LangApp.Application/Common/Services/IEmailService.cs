namespace LangApp.Application.Common.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendResetPasswordEmailAsync(string email, string token);
}
