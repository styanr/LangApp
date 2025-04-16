using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.StudyGroups;

public class GroupModificationPolicy : IGroupModificationPolicy
{
    public bool IsSatisfiedBy(StudyGroup group, ApplicationUser user)
    {
        return group.CanBeModifiedBy(user.Id);
    }
}