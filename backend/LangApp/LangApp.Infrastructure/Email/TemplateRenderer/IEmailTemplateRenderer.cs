namespace LangApp.Infrastructure.Email.TemplateRenderer;

public interface IEmailTemplateRenderer
{
    string RenderResetPasswordTemplate(string link);
}
