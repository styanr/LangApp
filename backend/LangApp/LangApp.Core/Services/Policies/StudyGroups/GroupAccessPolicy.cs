using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.StudyGroups;

public class GroupAccessPolicy : IGroupAccessPolicy
{
    public bool IsSatisfiedBy(StudyGroup assignment, ApplicationUser user)
    {
        return assignment.ContainsMember(user.Id) || assignment.CanBeModifiedBy(user.Id);
    }
}