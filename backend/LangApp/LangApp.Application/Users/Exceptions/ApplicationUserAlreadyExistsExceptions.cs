using LangApp.Core.Exceptions;

namespace LangApp.Application.Users.Exceptions;

public class ApplicationUserAlreadyExistsExceptions(string username)
    : LangAppException($"User with username/email '{username}' already exists");