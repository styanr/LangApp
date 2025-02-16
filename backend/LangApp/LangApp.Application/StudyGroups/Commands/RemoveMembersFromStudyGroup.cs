using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.StudyGroups.Commands;

public record RemoveMembersFromStudyGroup(
    Guid StudyGroupId,
    IEnumerable<Guid> Members
) : ICommand;

public class RemoveMembersFromStudyGroupHandler : ICommandHandler<RemoveMembersFromStudyGroup>
{
    private readonly IStudyGroupRepository _repository;

    public RemoveMembersFromStudyGroupHandler(IStudyGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveMembersFromStudyGroup command, CancellationToken cancellationToken)
    {
        var (studyGroupId, membersModel) = command;

        var members = membersModel.Select(m => new Member(m, studyGroupId));

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);
        studyGroup.RemoveMembers(members);
        await _repository.UpdateAsync(studyGroup);
    }
}