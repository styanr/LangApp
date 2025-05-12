using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Auth.Models;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Users.Services;
using LangApp.Core.Repositories;

namespace LangApp.Application.Auth.Commands;

public record Login(
    string Username,
    string Password
) : ICommand<TokenResponse>;

public class LoginHandler : ICommandHandler<Login, TokenResponse>
{
    private readonly IApplicationUserRepository _repository;
    private readonly IAuthService _authService;

    public LoginHandler(IApplicationUserRepository repository, IAuthService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    public async Task<TokenResponse> HandleAsync(Login command, CancellationToken cancellationToken)
    {
        var tokenResponse = await _authService.Authenticate(command.Username, command.Password);

        if (tokenResponse is null)
        {
            throw new InvalidCredentialsException();
        }

        return tokenResponse;
    }
}
