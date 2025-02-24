using LangApp.Core.Common.Exceptions;
using LangApp.Core.Exceptions;

namespace LangApp.Application.Users.Exceptions;

public class UserNotFoundException : NotFoundException
{
    public Guid Id { get; }

    public UserNotFoundException(Guid id) : base($"User with ID {id} was not found.")
    {
        Id = id;
    }
}