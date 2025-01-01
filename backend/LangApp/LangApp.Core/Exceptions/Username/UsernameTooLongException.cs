namespace LangApp.Core.Exceptions.Username;

public class UsernameTooLongException(string username, int maxLen)
    : LangAppException($"Username '{username} is longer than allowed length {maxLen}");