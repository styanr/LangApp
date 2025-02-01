using LangApp.Core.Exceptions;

namespace LangApp.Application.Users.Exceptions;

public class UserAlreadyExistsException(string username)
    : LangAppException($"User with username/email '{username}' already exists");