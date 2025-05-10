using LangApp.Core.Common.Exceptions;

namespace LangApp.Core.Exceptions.Posts;

public class CommentNotFoundException : NotFoundException
{
    public Guid Id { get; }

    public CommentNotFoundException(Guid id) : base($"Comment with id {id} not found")
    {
        Id = id;
    }
}
