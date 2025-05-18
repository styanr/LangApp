using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.Users;

namespace LangApp.Core.Services.Policies.Posts;

public class PostModificationPolicy : IPostModificationPolicy
{
    public bool IsSatisfiedBy(Post post, ApplicationUser user)
    {
        return post.CanBeModifiedBy(user.Id);
    }
}