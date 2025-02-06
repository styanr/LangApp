using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Posts.Commands;

public record CreatePost(
    Guid AuthorId,
    Guid GroupId,
    PostType Type,
    string Title,
    string Content,
    List<string>? Media = null
) : ICommand;

public class CreatePostHandler : ICommandHandler<CreatePost>
{
    private readonly IPostRepository _repository;
    private readonly IPostFactory _factory;

    public CreatePostHandler(IPostRepository repository, IPostFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task HandleAsync(CreatePost command, CancellationToken cancellationToken)
    {
        var content = new PostContent(command.Content);
        var post = _factory.Create(command.AuthorId, command.GroupId, command.Type, command.Title, content,
            command.Media ?? []);
        await _repository.AddAsync(post);
    }
}