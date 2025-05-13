using LangApp.Application.Common.Services;
using LangApp.Infrastructure.Email.DeepLinks;
using LangApp.Infrastructure.Email.TemplateRenderer;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LangApp.Infrastructure.Email.Options;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IOptions<SendGridOptions> _options;
    private readonly IDeepLinkGenerator _deepLinkGenerator;
    private readonly IEmailTemplateRenderer _emailTemplateRenderer;

    public SendGridEmailService(ISendGridClient sendGridClient, IOptions<SendGridOptions> options,
        IEmailTemplateRenderer emailTemplateRenderer, IDeepLinkGenerator deepLinkGenerator)
    {
        _sendGridClient = sendGridClient;
        _options = options;
        _emailTemplateRenderer = emailTemplateRenderer;
        _deepLinkGenerator = deepLinkGenerator;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var (senderEmail, senderName) = (_options.Value.SenderEmail, _options.Value.SenderName);
        var sendGridMessage = new SendGridMessage
        {
            From = new EmailAddress(senderEmail, senderName),
            Subject = subject,
            HtmlContent = message,
        };

        sendGridMessage.AddTo(new EmailAddress(email));
        await _sendGridClient.SendEmailAsync(sendGridMessage);
    }

    public async Task SendResetPasswordEmailAsync(string email, string token)
    {
        var link = _deepLinkGenerator.GenerateResetPasswordLink(email, token);
        var message = _emailTemplateRenderer.RenderResetPasswordTemplate(link);
        await SendEmailAsync(email, "Reset Password", message);
    }
}
