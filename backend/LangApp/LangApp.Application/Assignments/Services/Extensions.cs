using LangApp.Application.Assignments.Services.PolicyServices;
using LangApp.Application.Posts.Services.PolicyServices;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Assignments.Services;

public static class Extensions
{
    public static IServiceCollection AddAssignmentPolicies(this IServiceCollection services)
    {
        services.AddScoped<IAssignmentFullAccessPolicyService, AssignmentFullAccessPolicyService>();
        services.AddScoped<IAssignmentRestrictedAccessPolicyService, AssignmentRestrictedAccessPolicyService>();

        return services;
    }
}