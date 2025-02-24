using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record UnarchivePost(Guid Id) : ICommand;

public class UnarchivePostHandler : ICommandHandler<UnarchivePost>
{
    private readonly IPostRepository _repository;

    public UnarchivePostHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UnarchivePost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.Id, showArchived: true);
        if (post is null) throw new PostNotFoundException(command.Id);

        post.Archive();
        await _repository.UpdateAsync(post);
    }
}