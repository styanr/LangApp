using LangApp.Application.Common.Exceptions;
using LangApp.Core.Exceptions;

namespace LangApp.Application.Posts.Exceptions;

public class PostNotFoundException : NotFoundException
{
    public Guid CommandPostId { get; }

    public PostNotFoundException(Guid id) : base($"Post with ID {id} was not found.")
    {
        CommandPostId = id;
    }
}