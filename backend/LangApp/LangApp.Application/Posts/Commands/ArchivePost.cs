using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record ArchivePost(Guid PostId) : ICommand;

public class ArchivePostHandler : ICommandHandler<ArchivePost>
{
    private readonly IPostRepository _repository;

    public ArchivePostHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(ArchivePost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.PostId);
        if (post is null) throw new PostNotFoundException(command.PostId);

        post.Archive();
        await _repository.UpdateAsync(post);
    }
}