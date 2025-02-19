using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Users.Exceptions;
using LangApp.Application.Users.Models;
using LangApp.Application.Users.Services;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;
using Abstractions_ICommand = LangApp.Application.Common.Commands.Abstractions.ICommand;

namespace LangApp.Application.Users.Commands;

public record CreateUser(
    string Username,
    string Email,
    FullNameModel FullName,
    string? PictureUrl,
    AppUserRole Role
) : Abstractions_ICommand;

public class CreateUserHandler : ICommandHandler<CreateUser, Guid>
{
    private readonly IApplicationUserFactory _factory;
    private readonly IApplicationUserRepository _repository;
    private readonly IApplicationUserReadService _readService;

    public CreateUserHandler(IApplicationUserRepository repository, IApplicationUserReadService readService,
        IApplicationUserFactory factory)
    {
        _repository = repository;
        _readService = readService;
        _factory = factory;
    }

    public async Task<Guid> HandleAsync(CreateUser command, CancellationToken cancellationToken)
    {
        var (usernameModel, email, fullNameModel, pictureUrl, role) = command;
        if (await _readService.ExistsByUsernameAsync(command.Username))
        {
            throw new UserAlreadyExistsException(command.Username);
        }

        if (await _readService.ExistsByEmailAsync(command.Email))
        {
            throw new UserAlreadyExistsException(command.Email);
        }

        var username = new Username(usernameModel);
        var fullName = new UserFullName(fullNameModel.FirstName, fullNameModel.LastName);

        var user = _factory.Create(username, fullName, pictureUrl, role, email);
        await _repository.AddAsync(user);

        return user.Id;
    }
}