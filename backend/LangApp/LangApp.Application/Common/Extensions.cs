using System.Reflection;
using LangApp.Application.Assignments.Services;
using LangApp.Application.Auth.Options;
using LangApp.Application.Common.Commands;
using LangApp.Application.Common.DomainEvents;
using LangApp.Application.Common.Strategies;
using LangApp.Application.Posts.Services;
using LangApp.Application.StudyGroups.Services;
using LangApp.Core.Common;
using LangApp.Core.Factories.Assignments;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Factories.Submissions;
using LangApp.Core.Factories.Users;
using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.Services.KeyGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommands();
        services.AddDeeplinkConfiguration(configuration);
        var assembly = Assembly.GetAssembly(typeof(IGradingStrategy<>))!;

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IGradingStrategy<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IPolicy<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IPolicy<,,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.AddTransient<IKeyGenerator, KeyGenerator>();
        services.AddTransient<IApplicationUserFactory, ApplicationUserFactory>();
        services.AddTransient<IPostFactory, PostFactory>();
        services.AddTransient<IPostCommentFactory, PostCommentFactory>();
        services.AddTransient<IStudyGroupFactory, StudyGroupFactory>();
        services.AddTransient<IAssignmentFactory, AssignmentFactory>();
        services.AddTransient<IActivityFactory, ActivityFactory>();
        services.AddTransient<IActivitySubmissionFactory, ActivitySubmissionFactory>();
        services.AddTransient<IAssignmentSubmissionFactory, AssignmentSubmissionFactory>();

        // TODO move to separate methods
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IGradingStrategyDispatcher, InMemoryGradingStrategyDispatcher>();

        services.AddPostPolicies();
        services.AddStudyGroupServices();
        services.AddAssignmentPolicies();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
