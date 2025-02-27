using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Users.Models;
using LangApp.Application.Users.Services;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Users.Commands;

public record UpdateUserInfo(
    Guid Id,
    string Username,
    FullNameModel FullName,
    string? PictureUrl,
    UserRole UserRole) : ICommand;

public class UpdateUserInfoCommandHandler : ICommandHandler<UpdateUserInfo>
{
    // private readonly IApplicationUserRepository _repository;
    // private readonly IApplicationUserReadService _readService;
    //
    // public UpdateUserInfoCommandHandler(IApplicationUserFactory factory, IApplicationUserRepository repository,
    //     IApplicationUserReadService readService)
    // {
    //     _repository = repository;
    //     _readService = readService;
    // }
    //
    // public async Task HandleAsync(UpdateUserInfo command, CancellationToken cancellationToken)
    // {
    //     var (id, usernameModel, fullNameModel, pictureUrl, role) = command;
    //
    //     var user = await _repository.GetAsync(id) ?? throw new UserNotFoundException(id);
    //
    //     var username = new Username(usernameModel);
    //     var fullName = new UserFullName(fullNameModel.FirstName, fullNameModel.LastName);
    //
    //     if (!user.Username.Equals(username) && await _readService.ExistsByUsernameAsync(command.Username))
    //     {
    //         throw new UserAlreadyExistsException(command.Username);
    //     }
    //
    //     user.UpdateUsername(username);
    //     user.UpdateFullName(fullName);
    //     user.UpdatePictureUrl(pictureUrl);
    //     user.UpdateRole(role);
    //
    //     await _repository.UpdateAsync(user);
    // }
    public Task HandleAsync(UpdateUserInfo command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}