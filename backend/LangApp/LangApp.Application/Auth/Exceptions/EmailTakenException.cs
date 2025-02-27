using LangApp.Core.Exceptions;

namespace LangApp.Application.Auth.Exceptions;

public class EmailTakenException(string email)
    : LangAppException($"User with email '{email}' already exists.");