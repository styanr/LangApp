using LangApp.Application.Common.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace LangApp.Infrastructure.Email.Options;

public class SendGridEmailService : IEmailService
{
    private readonly ISendGridClient _sendGridClient;
    private readonly IOptions<SendGridOptions> _options;

    public SendGridEmailService(ISendGridClient sendGridClient, IOptions<SendGridOptions> options)
    {
        _sendGridClient = sendGridClient;
        _options = options;
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
}
