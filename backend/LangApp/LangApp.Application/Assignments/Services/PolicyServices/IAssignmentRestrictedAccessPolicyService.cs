namespace LangApp.Application.Assignments.Services.PolicyServices;

public interface IAssignmentRestrictedAccessPolicyService
{
    Task<bool> IsSatisfiedBy(Guid assignmentId, Guid studyGroupId, Guid userId);
}