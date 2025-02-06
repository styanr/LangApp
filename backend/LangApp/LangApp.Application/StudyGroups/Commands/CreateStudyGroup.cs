using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.StudyGroups.Commands;

public record CreateStudyGroup(
    string Name,
    string Language,
    Guid OwnerId
) : ICommand;

public class CreateStudyGroupHandler : ICommandHandler<CreateStudyGroup>
{
    private readonly IStudyGroupRepository _repository;
    private readonly IStudyGroupFactory _factory;

    public CreateStudyGroupHandler(IStudyGroupRepository repository, IStudyGroupFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task HandleAsync(CreateStudyGroup command, CancellationToken cancellationToken)
    {
        var (name, languageModel, ownerId) = command;

        var language = new Language(languageModel);

        var studyGroup = _factory.Create(name, language, ownerId);
        await _repository.AddAsync(studyGroup);
    }
}