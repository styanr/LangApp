namespace LangApp.Core.Common;

public interface IPolicy;

public interface IPolicy<TResource, TUser> : IPolicy
{
    bool IsSatisfiedBy(TResource resource, TUser user);
}

public interface IPolicy<TResource, TResource1, TUser> : IPolicy
{
    bool IsSatisfiedBy(TResource resource, TResource1 resource1, TUser user);
}