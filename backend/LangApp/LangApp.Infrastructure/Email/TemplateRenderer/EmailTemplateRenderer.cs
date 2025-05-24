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
}
