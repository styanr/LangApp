using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Exceptions.Posts;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record DeletePostComment(
    Guid PostId,
    Guid CommentId,
    Guid UserId) : ICommand;

public class DeletePostCommentHandler : ICommandHandler<DeletePostComment>
{
    private readonly IPostRepository _repository;

    public DeletePostCommentHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeletePostComment command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.PostId) ?? throw new PostNotFoundException(command.PostId);

        var comment = post.Comments.FirstOrDefault(c => c.Id == command.CommentId) ??
                      throw new CommentNotFoundException(command.CommentId);

        var isAllowed = comment.AuthorId == command.UserId;

        if (!isAllowed)
        {
            throw new UnauthorizedException(command.UserId, comment);
        }

        post.RemoveComment(command.CommentId);
        await _repository.UpdateAsync(post);
    }
}
