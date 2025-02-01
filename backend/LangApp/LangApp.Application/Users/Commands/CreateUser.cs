using LangApp.Application.Common.Abstractions;
using LangApp.Application.Users.Exceptions;
using LangApp.Application.Users.Models;
using LangApp.Core.Entities;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;
using ICommand = LangApp.Application.Common.Abstractions.ICommand;

namespace LangApp.Application.Users.Commands;

public record CreateUser(
    string Username,
    string Email,
    FullNameModel FullName,
    string? PictureUrl,
    AppUserRole Role
) : ICommand;

public class CreateUserHandler : ICommandHandler<CreateUser>
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

    public async Task HandleAsync(CreateUser command, CancellationToken cancellationToken)
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

        await _repository.AddAsync(_factory.Create(username, fullName, pictureUrl, role, email));
    }
}