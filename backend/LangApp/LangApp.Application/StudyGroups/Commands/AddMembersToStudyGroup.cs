using LangApp.Application.Auth.Exceptions;
using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.StudyGroups.Exceptions;
using LangApp.Application.Users.Services;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Enums;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.StudyGroups.Commands;

public record AddMembersToStudyGroup(
    Guid StudyGroupId,
    IEnumerable<Guid> Members,
    Guid UserId
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
        var (studyGroupId, membersModel, userId) = command;

        var members = membersModel.Select(m => new Member(m, studyGroupId)).ToList();

        var studyGroup = await _repository.GetAsync(studyGroupId) ??
                         throw new StudyGroupNotFound(studyGroupId);

        if (!studyGroup.CanBeModifiedBy(userId))
        {
            throw new UnauthorizedException(userId, studyGroup);
        }

        foreach (var member in members)
        {
            var exists = await _readService.Exists(member.UserId);
            if (!exists)
            {
                throw new UserNotFoundException(member.UserId);
            }

            var isStudent = await _readService.GetRoleAsync(member.UserId) == UserRole.Student;
            if (!isStudent)
            {
                throw new StudyGroupInvalidMemberException(studyGroupId);
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
