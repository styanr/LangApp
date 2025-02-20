using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Repositories;

namespace LangApp.Application.StudyGroups.Commands;

public record UpdateStudyGroupInfo(
    Guid StudyGroupId,
    string Name) : ICommand;

public class UpdateStudyGroupInfoCommandHandler : ICommandHandler<UpdateStudyGroupInfo>
{
    private readonly IStudyGroupRepository _repository;

    public UpdateStudyGroupInfoCommandHandler(IStudyGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateStudyGroupInfo command, CancellationToken cancellationToken)
    {
        var (studyGroupId, name) = command;

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);

        studyGroup.UpdateName(name);

        await _repository.UpdateAsync(studyGroup);
    }
}