namespace LangApp.Application.Assignments.Services.PolicyServices;

public interface IAssignmentFullAccessPolicyService
{
    Task<bool> IsSatisfiedBy(Guid assignmentId, Guid userId);
}