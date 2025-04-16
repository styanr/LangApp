using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.StudyGroups;

public class GroupAccessPolicy : IGroupAccessPolicy
{
    public bool IsSatisfiedBy(StudyGroup resource, ApplicationUser user)
    {
        return resource.ContainsMember(user.Id) || resource.CanBeModifiedBy(user.Id);
    }
}