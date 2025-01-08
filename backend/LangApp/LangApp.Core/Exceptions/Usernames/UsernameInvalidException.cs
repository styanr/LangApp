namespace LangApp.Core.Exceptions.Usernames;

public class UsernameInvalidException : LangAppException
{
    public string Username { get; }

    public UsernameInvalidException(string username) : base($"Username '{username} contains invalid characters'")
    {
        Username = username;
    }
}