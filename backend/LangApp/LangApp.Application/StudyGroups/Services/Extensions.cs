using LangApp.Application.Posts.Services.PolicyServices;
using LangApp.Application.StudyGroups.Services.PolicyServices;
using LangApp.Core.Services.Policies.Posts;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.StudyGroups.Services;

public static class Extensions
{
    public static IServiceCollection AddStudyGroupServices(this IServiceCollection services)
    {
        services.AddScoped<IStudyGroupAccessPolicyService, StudyGroupAccessPolicyService>();
        services.AddScoped<IStudyGroupModificationPolicyService, StudyGroupModificationPolicyService>();

        return services;
    }
}