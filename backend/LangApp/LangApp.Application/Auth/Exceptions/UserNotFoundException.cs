using LangApp.Core.Common.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public Guid Id { get; }

    public UserNotFoundException(Guid id) : base($"User with ID {id} was not found.")
    {
        Id = id;
    }
}