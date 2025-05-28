namespace LangApp.Infrastructure.Email.TemplateRenderer;

public interface IEmailTemplateRenderer
{
    string RenderResetPasswordTemplate(string link);
    string RenderConfirmationEmailTemplate(string link);
}
