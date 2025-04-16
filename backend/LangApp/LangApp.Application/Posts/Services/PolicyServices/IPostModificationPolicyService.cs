namespace LangApp.Application.Posts.Services.PolicyServices;

public interface IPostModificationPolicyService
{
    Task<bool> IsSatisfiedBy(Guid postId, Guid userId);
}