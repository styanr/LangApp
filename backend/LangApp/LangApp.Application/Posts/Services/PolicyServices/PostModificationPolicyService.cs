using LangApp.Core.Repositories;
using LangApp.Core.Services.Policies.Posts;

namespace LangApp.Application.Posts.Services.PolicyServices;

public class PostModificationPolicyService : IPostModificationPolicyService
{
    private readonly IPostRepository _postRepository;
    private readonly IApplicationUserRepository _userRepository;
    private readonly IPostModificationPolicy _policy;

    public PostModificationPolicyService(IPostRepository postRepository, IApplicationUserRepository userRepository,
        IPostModificationPolicy policy)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
        _policy = policy;
    }

    public async Task<bool> IsSatisfiedBy(Guid postId, Guid userId)
    {
        var post = await _postRepository.GetAsync(postId);
        var user = await _userRepository.GetAsync(userId);

        if (post is null || user is null)
        {
            return false;
        }

        return _policy.IsSatisfiedBy(post, user);
    }
}