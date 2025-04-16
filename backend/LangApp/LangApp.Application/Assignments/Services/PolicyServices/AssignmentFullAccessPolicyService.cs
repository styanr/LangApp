using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.Assignments;

namespace LangApp.Application.Assignments.Services.PolicyServices;

public class AssignmentFullAccessPolicyService : IAssignmentFullAccessPolicyService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IAssignmentFullAccessPolicy _policy;

    public AssignmentFullAccessPolicyService(IAssignmentRepository assignmentRepository,
        IApplicationUserRepository userRepository, IAssignmentFullAccessPolicy policy)
    {
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _policy = policy;
    }

    public async Task<bool> IsSatisfiedBy(Guid assignmentId, Guid userId)
    {
        var assignment = await _assignmentRepository.GetAsync(assignmentId);
        var user = await _userRepository.GetAsync(userId);

        if (assignment is null || user is null)
        {
            return false;
        }

        return _policy.IsSatisfiedBy(assignment, user);
    }
}