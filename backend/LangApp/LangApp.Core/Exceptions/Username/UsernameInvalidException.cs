namespace LangApp.Core.Exceptions.Username;

public class UsernameInvalidException(string username)
    : LangAppException($"Username '{username} contains invalid characters'");