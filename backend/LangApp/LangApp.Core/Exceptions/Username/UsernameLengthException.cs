namespace LangApp.Core.Exceptions.Username;

public class UsernameLengthException : LangAppException
{
    public string Username { get; }
    public int Length { get; }
    public int MinLength { get; }
    public int MaxLength { get; }

    public UsernameLengthException(string username, int length, int minLength, int maxLength) : base(
        $"Username length of {username} is invalid ({length}). It must be between {minLength} and {maxLength}")
    {
        Username = username;
        Length = length;
        MinLength = minLength;
        MaxLength = maxLength;
    }
}