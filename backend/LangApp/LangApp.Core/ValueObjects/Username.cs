using System.Text.RegularExpressions;
using LangApp.Core.Common;

namespace LangApp.Core.ValueObjects;

public record Username
{
    private const int MinLen = 4;
    private const int MaxLen = 20;
    private readonly Regex re = new("^(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$");

    public string Value { get; }

    // can maybe create a single exception with a reason
    public Username(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new UsernameEmptyException();
        }

        if (username.Length < MinLen)
        {
            throw new UsernameTooShortException(username, MinLen);
        }

        if (username.Length > MaxLen)
        {
            throw new UsernameTooLongException(username, MaxLen);
        }

        if (!re.IsMatch(username))
        {
            throw new UsernameInvalidException(username);
        }
    }
};