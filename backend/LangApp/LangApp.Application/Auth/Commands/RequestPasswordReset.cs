using System.Net;
using LangApp.Application.Auth.Options;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LangApp.Application.Auth.Commands;

public record RequestPasswordReset(
    string Email
) : ICommand;

public class RequestPasswordResetHandler : ICommandHandler<RequestPasswordReset>
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly ILogger<RequestPasswordResetHandler> _logger;

    public RequestPasswordResetHandler(IAuthService authService,
        IEmailService emailService, ILogger<RequestPasswordResetHandler> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task HandleAsync(RequestPasswordReset command, CancellationToken cancellationToken)
    {
        var resetToken = await _authService.RequestPasswordReset(command.Email);

        if (resetToken is null)
        {
            _logger.LogWarning("Password reset requested for unknown user: {Email}", command.Email);
            return;
        }

        await _emailService.SendResetPasswordEmailAsync(command.Email, resetToken);
        _logger.LogInformation("Password reset notification sent to {Email}", command.Email);
    }
}
