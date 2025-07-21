using System.Text.RegularExpressions;
using LangApp.Core.Common;
using LangApp.Core.Exceptions.Emails;

namespace LangApp.Core.ValueObjects;

public record Email
{
    private const int MaxLength = 254; // RFC 5321 limit

    private readonly Regex re = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.None,
        TimeSpan.FromSeconds(1.5));

    public string Value { get; }

    public Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new EmailEmptyException();
        }

        if (email.Length > MaxLength)
        {
            throw new EmailTooLongException(email, email.Length, MaxLength);
        }

        if (!re.IsMatch(email))
        {
            throw new EmailInvalidException(email);
        }

        Value = email.ToLowerInvariant(); // Normalize email to lowercase
    }

    public override string ToString()
    {
        return Value;
    }

    public virtual bool Equals(Email? other)
    {
        return other?.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}