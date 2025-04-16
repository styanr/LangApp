using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.StudyGroups;

namespace LangApp.Application.StudyGroups.Services.PolicyServices;

public class StudyGroupAccessPolicyService : IStudyGroupAccessPolicyService
{
    private readonly IStudyGroupRepository _groupRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IGroupAccessPolicy _policy;

    public StudyGroupAccessPolicyService(
        IStudyGroupRepository groupRepository,
        IGroupAccessPolicy policy,
        IApplicationUserRepository userRepository)
    {
        _groupRepository = groupRepository;
        _policy = policy;
        _userRepository = userRepository;
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