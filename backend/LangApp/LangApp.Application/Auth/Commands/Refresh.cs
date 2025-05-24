using LangApp.Application.Auth.Models;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;

namespace LangApp.Application.Auth.Commands;

public record Refresh(
    string RefreshToken) : ICommand<TokenResponse>;

class RefreshHandler : ICommandHandler<Refresh, TokenResponse>
{
    private readonly IAuthService _authService;

    public RefreshHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<TokenResponse> HandleAsync(Refresh command, CancellationToken cancellationToken)
    {
        var tokenResponse = await _authService.RefreshToken(command.RefreshToken);

        return tokenResponse;
    }
}
