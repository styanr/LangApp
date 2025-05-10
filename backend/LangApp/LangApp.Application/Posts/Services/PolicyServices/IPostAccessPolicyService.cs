namespace LangApp.Application.Posts.Services.PolicyServices;

public interface IPostAccessPolicyService
{
    Task<bool> IsSatisfiedBy(Guid postId, Guid userId);
}
