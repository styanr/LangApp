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
    private readonly IApplicationUserRepository _userRepository;

    public AddMembersToStudyGroupHandler(IStudyGroupRepository repository, IApplicationUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task HandleAsync(AddMembersToStudyGroup command, CancellationToken cancellationToken)
    {
        var (studyGroupId, membersModel) = command;

        var members = membersModel.Select(m => new Member(m, studyGroupId)).ToList();

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);

        if (members.Any(m => m.UserId == studyGroupId))
        {
            throw new StudyGroupInvalidMemberException(studyGroupId);
        }

        studyGroup.AddMembers(members);
        await _repository.UpdateAsync(studyGroup);
    }
}