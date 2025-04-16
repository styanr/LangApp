using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Core.Services.Policies.Posts;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Posts.Services;

public static class Extensions
{
    public static IServiceCollection AddPostPolicies(this IServiceCollection services)
    {
        services.AddScoped<IPostAccessPolicyService, PostAccessPolicyService>();
        services.AddScoped<IPostModificationPolicyService, PostModificationPolicyService>();

        return services;
    }
}