using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Application.Users.Models;
using LangApp.Application.Users.Services;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
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
    private readonly IApplicationUserRepository _repository;
    private readonly IApplicationUserReadService _readService;
    private readonly IJobScheduler _jobScheduler;
    private readonly IEmailService _emailService;

    public RegisterHandler(
        IApplicationUserFactory factory,
        IApplicationUserRepository repository,
        IApplicationUserReadService readService, IJobScheduler jobScheduler, IEmailService emailService)
    {
        _factory = factory;
        _repository = repository;
        _readService = readService;
        _jobScheduler = jobScheduler;
        _emailService = emailService;
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
        await _repository.AddAsync(user, password);

        _jobScheduler.Enqueue(() => _emailService.SendEmailAsync(user.Email, "Welcome to LangApp",
            "Thank you for registering to LangApp. Happy trails!"));

        return user.Id;
    }
}
