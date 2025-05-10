using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Exceptions.Posts;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record EditPostComment(
    Guid PostId,
    Guid CommentId,
    string Content,
    Guid UserId
) : ICommand;

public class EditPostCommentHandler : ICommandHandler<EditPostComment>
{
    private readonly IPostRepository _repository;

    public EditPostCommentHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditPostComment command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.PostId) ?? throw new PostNotFoundException(command.PostId);

        var comment = post.Comments.FirstOrDefault(c => c.Id == command.CommentId) ??
                      throw new CommentNotFoundException(command.CommentId);

        var isAllowed = comment.AuthorId == command.UserId;

        if (!isAllowed)
        {
            throw new UnauthorizedException(command.UserId, comment);
        }

        post.UpdateComment(command.CommentId, command.Content);
        await _repository.UpdateAsync(post);
    }
}
