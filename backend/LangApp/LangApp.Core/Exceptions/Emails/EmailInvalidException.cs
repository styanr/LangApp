namespace LangApp.Core.Exceptions.Emails;

public class EmailInvalidException : LangAppException
{
    public string Email { get; }

    public EmailInvalidException(string email) : base($"Email '{email}' has an invalid format")
    {
        Email = email;
    }
}