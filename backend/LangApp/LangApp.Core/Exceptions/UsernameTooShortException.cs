namespace LangApp.Core.Common;

public class UsernameTooShortException(string username, int minLen)
    : LangAppException($"Username '{username} is shorter than allowed length {minLen}");