using LangApp.Core.Entities.Posts;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Entities.Users;
using LangApp.Core.Services.Policies.StudyGroups;

namespace LangApp.Core.Services.Policies.Posts;

public class PostAccessPolicy : IPostAccessPolicy
{
    private readonly IGroupAccessPolicy _groupAccessPolicy;

    public PostAccessPolicy(IGroupAccessPolicy groupAccessPolicy)
    {
        _groupAccessPolicy = groupAccessPolicy;
    }

    public bool IsSatisfiedBy(Post post, StudyGroup group, ApplicationUser user)
    {
        return post.GroupId == group.Id && _groupAccessPolicy.IsSatisfiedBy(group, user);
    }
}