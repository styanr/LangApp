using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Posts.Exceptions;
using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Repositories;

namespace LangApp.Application.Posts.Commands;

public record CreatePostComment(
    Guid AuthorId,
    Guid PostId,
    string Content
) : ICommand<Guid>;

public class CreatePostCommentHandler : ICommandHandler<CreatePostComment, Guid>
{
    private readonly IPostRepository _repository;
    private readonly IPostCommentFactory _factory;
    private readonly IPostAccessPolicyService _policy;

    public CreatePostCommentHandler(IPostRepository repository, IPostCommentFactory factory,
        IPostAccessPolicyService policy)
    {
        _repository = repository;
        _factory = factory;
        _policy = policy;
    }

    public async Task<Guid> HandleAsync(CreatePostComment command, CancellationToken cancellationToken)
    {
        var post = await _repository.GetAsync(command.PostId) ?? throw new PostNotFoundException(command.PostId);

        var isAllowedToPost = await _policy.IsSatisfiedBy(post.Id, command.AuthorId);
        if (!isAllowedToPost)
        {
            throw new UnauthorizedException(command.AuthorId, post);
        }

        var comment = _factory.Create(command.AuthorId, post.Id, command.Content);

        post.AddComment(comment);
        await _repository.UpdateAsync(post);

        return comment.Id;
    }
}
