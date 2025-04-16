using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.StudyGroups;

namespace LangApp.Application.StudyGroups.Services.PolicyServices;

public class StudyGroupModificationPolicyService : IStudyGroupModificationPolicyService
{
    private readonly IStudyGroupRepository _groupRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IGroupModificationPolicy _policy;

    public StudyGroupModificationPolicyService(
        IApplicationUserRepository userRepository,
        IStudyGroupRepository groupRepository, IGroupModificationPolicy policy)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _policy = policy;
    }

    public async Task<bool> IsSatisfiedBy(Guid studyGroupId, Guid userId)
    {
        var group = await _groupRepository.GetAsync(studyGroupId);
        var user = await _userRepository.GetAsync(userId);

        if (group is null || user is null)
        {
            return false;
        }

        return _policy.IsSatisfiedBy(group, user);
    }
}