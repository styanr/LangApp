using LangApp.Core.Enums;
using LangApp.Core.Exceptions;

namespace LangApp.Application.Common.Exceptions;

public class UnauthorizedRoleException<T> : LangAppException
{
    public Guid UserId { get; set; }
    public UserRole Role { get; set; }

    public UnauthorizedRoleException(Guid userId, UserRole role) : base(
        $"The user with the ID {userId} and the role {role} is not allowed to perform the action on {typeof(T).Name}")
    {
        Role = role;
        UserId = userId;
    }
}
