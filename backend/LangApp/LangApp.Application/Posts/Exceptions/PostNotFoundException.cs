using LangApp.Core.Exceptions;

namespace LangApp.Application.Posts.Exceptions;

public class PostNotFoundException : LangAppException
{
    public Guid CommandPostId { get; }

    public PostNotFoundException(Guid id) : base($"Post with ID {id} was not found.")
    {
        CommandPostId = id;
    }
}