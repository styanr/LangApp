using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Repositories;

namespace LangApp.Application.StudyGroups.Commands;

public record UpdateStudyGroupInfo(
    Guid StudyGroupId,
    string Name,
    Guid UserId
) : ICommand;

public class UpdateStudyGroupInfoCommandHandler : ICommandHandler<UpdateStudyGroupInfo>
{
    private readonly IStudyGroupRepository _repository;

    public UpdateStudyGroupInfoCommandHandler(IStudyGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateStudyGroupInfo command, CancellationToken cancellationToken)
    {
        var (studyGroupId, name, userId) = command;

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);

        if (!studyGroup.CanBeModifiedBy(userId))
        {
            throw new UnauthorizedException(userId, studyGroup);
        }

        studyGroup.UpdateName(name);

        await _repository.UpdateAsync(studyGroup);
    }
}