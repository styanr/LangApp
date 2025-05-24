using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Enums;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.StudyGroups.Commands;

public record CreateStudyGroup(
    string Name,
    string Language,
    Guid OwnerId,
    UserRole OwnerRole
) : ICommand<Guid>;

public class CreateStudyGroupHandler : ICommandHandler<CreateStudyGroup, Guid>
{
    private readonly IStudyGroupRepository _repository;
    private readonly IStudyGroupFactory _factory;

    public CreateStudyGroupHandler(IStudyGroupRepository repository, IStudyGroupFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task<Guid> HandleAsync(CreateStudyGroup command, CancellationToken cancellationToken)
    {
        var (name, languageModel, ownerId, role) = command;

        if (role != UserRole.Teacher)
        {
            throw new UnauthorizedRoleException<StudyGroup>(ownerId, role);
        }

        var language = Language.FromString(languageModel);

        var studyGroup = _factory.Create(name, language, ownerId);
        await _repository.AddAsync(studyGroup);

        return studyGroup.Id;
    }
}
