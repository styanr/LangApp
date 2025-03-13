using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Entities.StudyGroups;
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
) : ICommand<Guid>;

public class CreatePostHandler : ICommandHandler<CreatePost, Guid>
{
    private readonly IPostRepository _repository;
    private readonly IPostFactory _factory;
    private readonly IStudyGroupRepository _studyGroupRepository;

    public CreatePostHandler(
        IPostRepository repository,
        IPostFactory factory,
        IStudyGroupRepository studyGroupRepository
    )
    {
        _repository = repository;
        _factory = factory;
        _studyGroupRepository = studyGroupRepository;
    }

    public async Task<Guid> HandleAsync(CreatePost command, CancellationToken cancellationToken)
    {
        var group = await _studyGroupRepository.GetAsync(command.GroupId) ??
                    throw new StudyGroupNotFoundException(command.GroupId);

        if (!group.ContainsMember(command.AuthorId) && group.OwnerId != command.AuthorId)
        {
            throw new UnauthorizedException(command.AuthorId, group);
        }

        var content = new PostContent(command.Content);
        var post = _factory.Create(command.AuthorId, command.GroupId, command.Type, command.Title, content,
            command.Media ?? []);
        await _repository.AddAsync(post);

        return post.Id;
    }
}