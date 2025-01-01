namespace LangApp.Core.Common;

public class UsernameTooLongException(string username, int maxLen)
    : LangAppException($"Username '{username} is longer than allowed length {maxLen}");