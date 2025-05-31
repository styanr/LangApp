using System.Text.RegularExpressions;
using LangApp.Core.Common;
using LangApp.Core.Exceptions.Usernames;

namespace LangApp.Core.ValueObjects;

public record Username
{
    private const int MinLength = 4;
    private const int MaxLength = 20;

    private readonly Regex re = new("^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$", RegexOptions.None,
        TimeSpan.FromSeconds(1.5));

    public string Value { get; }

    // can maybe create a single exception with a reason
    public Username(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new UsernameEmptyException();
        }

        if (username.Length is < MinLength or > MaxLength)
        {
            throw new UsernameLengthException(username, username.Length, MinLength, MaxLength);
        }

        if (!re.IsMatch(username))
        {
            throw new UsernameInvalidException(username);
        }

        Value = username;
    }

    public override string ToString()
    {
        return Value;
    }

    public virtual bool Equals(Username? other)
    {
        return other?.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
};