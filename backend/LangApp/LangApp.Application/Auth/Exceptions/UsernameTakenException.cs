using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class UsernameTakenException(string username)
    : LangAppException($"User with username '{username}' already exists.");