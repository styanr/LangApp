namespace LangApp.Infrastructure.Email.DeepLinks;

public interface IDeepLinkGenerator
{
    string GenerateResetPasswordLink(string email, string token);
    string GenerateConfirmationLink(string email, string token);
}
