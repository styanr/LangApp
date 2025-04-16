using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Assignments;

public class AssignmentFullAccessPolicy : IAssignmentFullAccessPolicy
{
    public bool IsSatisfiedBy(Assignment assignment, ApplicationUser user)
    {
        return assignment.CanBeFullyAccessedBy(user.Id);
    }
}