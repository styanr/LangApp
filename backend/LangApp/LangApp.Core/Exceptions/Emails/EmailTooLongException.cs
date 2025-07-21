namespace LangApp.Core.Exceptions.Emails;

public class EmailTooLongException : LangAppException
{
    public string Email { get; }
    public int Length { get; }
    public int MaxLength { get; }

    public EmailTooLongException(string email, int length, int maxLength) : base(
        $"Email length of {email} is too long ({length}). Maximum allowed length is {maxLength}")
    {
        Email = email;
        Length = length;
        MaxLength = maxLength;
    }
}