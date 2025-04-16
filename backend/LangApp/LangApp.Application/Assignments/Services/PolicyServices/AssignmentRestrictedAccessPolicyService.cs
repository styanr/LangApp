using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.Assignments;

namespace LangApp.Application.Assignments.Services.PolicyServices;

public class AssignmentRestrictedAccessPolicyService : IAssignmentRestrictedAccessPolicyService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IStudyGroupRepository _groupRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IAssignmentRestrictedPolicy _policy;


    public AssignmentRestrictedAccessPolicyService(IAssignmentRepository assignmentRepository,
        IStudyGroupRepository groupRepository, IApplicationUserRepository userRepository,
        IAssignmentRestrictedPolicy policy)
    {
        _assignmentRepository = assignmentRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _policy = policy;
    }

    public async Task<bool> IsSatisfiedBy(Guid assignmentId, Guid studyGroupId, Guid userId)
    {
        var assignment = await _assignmentRepository.GetAsync(assignmentId);
        var group = await _groupRepository.GetAsync(studyGroupId);
        var user = await _userRepository.GetAsync(userId);

        if (assignment is null || group is null || user is null)
        {
            return false;
        }

        return _policy.IsSatisfiedBy(assignment, group, user);
    }
}