using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record UnarchivePost(Guid Id, Guid UserId) : ICommand;

public class UnarchivePostHandler : ICommandHandler<UnarchivePost>
{
    private readonly IPostRepository _repository;
    private readonly IPostModificationPolicyService _policy;

    public UnarchivePostHandler(IPostRepository repository, IPostModificationPolicyService policy)
    {
        _repository = repository;
        _policy = policy;
    }

    public async Task HandleAsync(UnarchivePost command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.Id, showArchived: true) ??
                   throw new PostNotFoundException(command.Id);

        var isAllowed = await _policy.IsSatisfiedBy(command.Id, command.UserId);
        if (!isAllowed)
        {
            throw new UnauthorizedException(command.UserId, post);
        }

        post.Archive();
        await _repository.UpdateAsync(post);
    }
}