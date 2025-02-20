using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Application.Users.Exceptions;
using LangApp.Application.Users.Services;
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
    private readonly IApplicationUserReadService _readService;

    public AddMembersToStudyGroupHandler(IStudyGroupRepository repository, IApplicationUserRepository userRepository,
        IApplicationUserReadService readService)
    {
        _repository = repository;
        _userRepository = userRepository;
        _readService = readService;
    }

    public async Task HandleAsync(AddMembersToStudyGroup command, CancellationToken cancellationToken)
    {
        var (studyGroupId, membersModel) = command;

        var members = membersModel.Select(m => new Member(m, studyGroupId)).ToList();

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFoundException(studyGroupId);

        foreach (var member in members)
        {
            var exists = await _readService.Exists(member.UserId);
            if (!exists)
            {
                throw new UserNotFoundException(member.UserId);
            }
        }

        if (members.Any(m => m.UserId == studyGroup.OwnerId))
        {
            throw new StudyGroupInvalidMemberException(studyGroupId);
        }

        studyGroup.AddMembers(members);
        await _repository.UpdateAsync(studyGroup);
    }
}