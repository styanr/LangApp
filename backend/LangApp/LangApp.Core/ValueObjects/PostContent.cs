using LangApp.Core.Exceptions;

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
    }
}

public class PostContentLengthException : LangAppException
{
    public int ContentLength { get; }
    public int MinLength { get; }
    public int MaxLength { get; }

    public PostContentLengthException(int contentLength, int minLength, int maxLength) : base(
        $"Content length of {contentLength} is invalid. It must be between {minLength} and {maxLength}")
    {
        ContentLength = contentLength;
        MinLength = minLength;
        MaxLength = maxLength;
    }
}