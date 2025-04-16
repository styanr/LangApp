using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.Posts;

namespace LangApp.Application.Posts.Services.PolicyServices;

public class PostAccessPolicyService : IPostAccessPolicyService
{
    private readonly IPostRepository _postRepository;
    private readonly IStudyGroupRepository _groupRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IPostAccessPolicy _policy;

    public PostAccessPolicyService(
        IPostRepository postRepository,
        IStudyGroupRepository groupRepository,
        IApplicationUserRepository userRepository,
        IPostAccessPolicy policy)
    {
        _postRepository = postRepository;
        _groupRepository = groupRepository;
        _userRepository = userRepository;
        _policy = policy;
    }

    public async Task<bool> IsSatisfiedBy(Guid postId, Guid groupId, Guid userId)
    {
        var post = await _postRepository.GetAsync(postId);
        var group = await _groupRepository.GetAsync(groupId);
        var user = await _userRepository.GetAsync(userId);

        if (post is null || group is null || user is null)
        {
            return false;
        }

        return _policy.IsSatisfiedBy(post, group, user);
    }
}