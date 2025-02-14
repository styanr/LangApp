using LangApp.Core.Exceptions;
using LangApp.Core.Exceptions.Posts;

namespace LangApp.Core.ValueObjects;

public record PostContent
{
    private const int MinLength = 20;
    private const int MaxLength = 500;
    public string Value { get; private set; }

    public PostContent(string content)
    {
        if (content.Length is < MinLength or > MaxLength)
        {
            throw new PostContentLengthException(content.Length, MinLength, MaxLength);
        }

        Value = content;
    }

    public override string ToString()
    {
        return Value;
    }
}