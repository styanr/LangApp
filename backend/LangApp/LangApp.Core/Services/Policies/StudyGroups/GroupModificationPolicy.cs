using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.StudyGroups;

public class GroupModificationPolicy : IGroupModificationPolicy
{
    public bool IsSatisfiedBy(StudyGroup assignment, ApplicationUser user)
    {
        return assignment.CanBeModifiedBy(user.Id);
    }
}