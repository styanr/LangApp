using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Posts.Commands;

public record EditPost(
    Guid Id,
    string Content,
    List<string>? Media,
    Guid UserId
) : ICommand;

public class EditPostHandler : ICommandHandler<EditPost>
{
    private readonly IPostRepository _repository;
    private readonly IPostModificationPolicyService _policy;

    public EditPostHandler(IPostRepository repository, IPostModificationPolicyService policy)
    {
        _repository = repository;
        _policy = policy;
    }

    public async Task HandleAsync(EditPost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.Id);
        var content = new PostContent(command.Content);

        if (post is null) throw new PostNotFoundException(command.Id);

        var isAllowed = await _policy.IsSatisfiedBy(command.Id, command.UserId);
        if (!isAllowed)
        {
            throw new UnauthorizedException(command.UserId, post);
        }

        post.Edit(content, command.Media ?? new List<string>());
        await _repository.UpdateAsync(post);
    }
}
