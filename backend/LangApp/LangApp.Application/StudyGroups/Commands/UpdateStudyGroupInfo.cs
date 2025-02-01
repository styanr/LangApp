using System.Windows.Input;
using LangApp.Application.Common.Abstractions;
using LangApp.Core.Repositories;
using ICommand = LangApp.Application.Common.Abstractions.ICommand;

namespace LangApp.Application.StudyGroups.Commands;

public record UpdateStudyGroupInfo(
    Guid StudyGroupId,
    string Name) : ICommand;

public class UpdateStudyGroupInfoCommandHandler : ICommandHandler<UpdateStudyGroupInfo>
{
    private IStudyGroupRepository _repository;

    public UpdateStudyGroupInfoCommandHandler(IStudyGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdateStudyGroupInfo command, CancellationToken cancellationToken)
    {
        var (id, name) = command;

        var studyGroup = await _repository.GetAsync(id);

        studyGroup.UpdateName(name);

        await _repository.UpdateAsync(studyGroup);
    }
}