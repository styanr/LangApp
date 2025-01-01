namespace LangApp.Core.Common;

public class UsernameInvalidException(string username)
    : LangAppException($"Username '{username} contains invalid characters'");