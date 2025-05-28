namespace LangApp.Infrastructure.Email.TemplateRenderer;

public class EmailTemplateRenderer : IEmailTemplateRenderer
{
    public string RenderResetPasswordTemplate(string link)
    {
        return $"""
                You can reset your password by clicking on the link below.<br />
                <a href="{link}">Reset Password</a>
                """;
    }

    public string RenderConfirmationEmailTemplate(string link)
    {
        return $"""
                Welcome to LangApp! Please confirm your email address by clicking the link below.<br />
                <a href="{link}">Confirm Email</a>
                """;
    }
}
