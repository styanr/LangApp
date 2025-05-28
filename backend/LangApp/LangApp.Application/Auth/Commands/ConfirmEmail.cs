using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;

namespace LangApp.Application.Auth.Commands;

public record ConfirmEmail(
    string Email,
    string Token
) : ICommand;

public class ConfirmEmailHandler : ICommandHandler<ConfirmEmail>
{
    private readonly IAuthService _authService;

    public ConfirmEmailHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task HandleAsync(ConfirmEmail command, CancellationToken cancellationToken)
    {
        await _authService.ConfirmEmailAsync(command.Email, command.Token);
    }
}