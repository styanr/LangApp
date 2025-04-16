using LangApp.Core.Entities.Assignments;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Assignments;

public class AssignmentRestrictedPolicy : IAssignmentRestrictedPolicy
{
    public bool IsSatisfiedBy(Assignment assignment, StudyGroup studyGroup, ApplicationUser user)
    {
        return studyGroup.ContainsMember(user.Id) || assignment.CanBeFullyAccessedBy(user.Id);
    }
}