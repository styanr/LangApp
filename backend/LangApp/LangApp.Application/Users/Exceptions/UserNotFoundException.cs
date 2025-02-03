using LangApp.Core.Exceptions;

namespace LangApp.Application.Users.Exceptions;

public class UserNotFoundException : LangAppException
{
    public Guid Id { get; }

    public UserNotFoundException(Guid id) : base($"User with ID {id} was not found.")
    {
        Id = id;
    }
}