namespace LangApp.Core.Exceptions.Posts;

public class PostCommentLengthException : LangAppException
{
    public int ContentLength { get; }
    public int MaxLength { get; }

    public PostCommentLengthException(int contentLength, int maxLength) : base(
        $"Comment length of {contentLength} is invalid. It must be less than {maxLength}")
    {
        ContentLength = contentLength;
        MaxLength = maxLength;
    }
}