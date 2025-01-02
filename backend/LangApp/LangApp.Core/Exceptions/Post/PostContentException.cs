namespace LangApp.Core.Exceptions.Post;

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