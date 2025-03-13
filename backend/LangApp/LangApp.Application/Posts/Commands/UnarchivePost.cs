using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record UnarchivePost(Guid Id, Guid UserId) : ICommand;

public class UnarchivePostHandler : ICommandHandler<UnarchivePost>
{
    private readonly IPostRepository _repository;

    public UnarchivePostHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UnarchivePost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.Id, showArchived: true) ??
                   throw new PostNotFoundException(command.Id);

        if (!post.CanBeModifiedBy(command.UserId))
        {
            throw new UnauthorizedException(command.UserId, post);
        }

        post.Archive();
        await _repository.UpdateAsync(post);
    }
}