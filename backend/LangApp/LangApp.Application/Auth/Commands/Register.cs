using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Auth.Services;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.Users.Models;
using LangApp.Application.Users.Services;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Auth.Commands;

public record Register(
    string Username,
    string Email,
    FullNameModel FullName,
    string? PictureUrl,
    UserRole Role,
    string Password
) : ICommand<Guid>;

public class RegisterHandler : ICommandHandler<Register, Guid>
{
    private readonly IApplicationUserFactory _factory;
    private readonly IApplicationUserReadService _readService;
    private readonly IJobScheduler _jobScheduler;
    private readonly IEmailService _emailService;
    private readonly IAuthService _authService;

    public RegisterHandler(
        IApplicationUserFactory factory,
        IApplicationUserReadService readService,
        IJobScheduler jobScheduler,
        IEmailService emailService,
        IAuthService authService)
    {
        _factory = factory;
        _readService = readService;
        _jobScheduler = jobScheduler;
        _emailService = emailService;
        _authService = authService;
    }

    public async Task<Guid> HandleAsync(Register command, CancellationToken cancellationToken)
    {
        var (usernameModel, email, fullNameModel, pictureUrl, role, password) = command;

        if (await _readService.ExistsByUsernameAsync(command.Username))
        {
            throw new UsernameTakenException(command.Username);
        }

        if (await _readService.ExistsByEmailAsync(command.Email))
        {
            throw new EmailTakenException(command.Email);
        }

        var username = new Username(usernameModel);
        var fullName = new UserFullName(fullNameModel.FirstName, fullNameModel.LastName);

        var user = _factory.Create(username, fullName, pictureUrl, role, email);
        await _authService.CreateUserAsync(user, password);

        var token = await _authService.GenerateEmailConfirmationTokenAsync(email);
        _jobScheduler.Enqueue(() => _emailService.SendConfirmationEmailAsync(email, token));

        return user.Id;
    }
}
