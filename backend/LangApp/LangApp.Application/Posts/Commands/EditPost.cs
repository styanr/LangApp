using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Posts.Commands;

public record EditPost(
    Guid PostId,
    string Content
) : ICommand;

public class EditPostHandler : ICommandHandler<EditPost>
{
    private readonly IPostRepository _repository;

    public EditPostHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(EditPost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.PostId);
        var content = new PostContent(command.Content);

        if (post is null) throw new PostNotFoundException(command.PostId);

        post.Edit(content);
        await _repository.UpdateAsync(post);
    }
}