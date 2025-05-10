using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Posts;

public class PostModificationPolicy : IPostModificationPolicy
{
    public bool IsSatisfiedBy(Post assignment, ApplicationUser user)
    {
        return assignment.CanBeModifiedBy(user.Id);
    }
}