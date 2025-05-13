using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;

namespace LangApp.Application.Auth.Commands;

public record ResetPassword(
    string Email,
    string Token,
    string NewPassword
) : ICommand;

public class ResetPasswordHandler : ICommandHandler<ResetPassword>
{
    private readonly IAuthService _authService;

    public ResetPasswordHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task HandleAsync(ResetPassword command, CancellationToken cancellationToken)
    {
        var (email, token, password) = command;

        var success = await _authService.ResetPasswordAsync(email, token, password);

        if (!success)
        {
            throw new InvalidCredentialsException("Password reset failed");
        }
    }
}
