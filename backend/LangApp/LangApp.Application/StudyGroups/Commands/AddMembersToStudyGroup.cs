using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.StudyGroups.Commands;

public record AddMembersToStudyGroup(
    Guid StudyGroupId,
    IEnumerable<Guid> Members
) : ICommand;

public class AddMembersToStudyGroupHandler : ICommandHandler<AddMembersToStudyGroup>
{
    private readonly IStudyGroupRepository _repository;

    public AddMembersToStudyGroupHandler(IStudyGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddMembersToStudyGroup command, CancellationToken cancellationToken)
    {
        var (studyGroupId, membersModel) = command;

        var members = membersModel.Select(m => new Member(m));

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);
        studyGroup.AddMembers(members);
        await _repository.UpdateAsync(studyGroup);
    }
}