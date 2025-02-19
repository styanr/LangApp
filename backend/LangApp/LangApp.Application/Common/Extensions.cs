using System.Reflection;
using LangApp.Application.Common.Commands;
using LangApp.Application.Common.Queries;
using LangApp.Application.Users.Services;
using LangApp.Core.Factories.Lexicons;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Factories.Users;
using LangApp.Core.Services.EvaluationStrategies;
using LangApp.Core.Services.KeyGeneration;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common;

public static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddCommands();
        var assembly = Assembly.GetAssembly(typeof(IEvaluationStrategy<,>))!;

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEvaluationStrategy<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddTransient<IKeyGenerator, KeyGenerator>();
        services.AddTransient<IApplicationUserFactory, ApplicationUserFactory>();
        services.AddTransient<ILexiconFactory, LexiconFactory>();
        services.AddTransient<ILexiconEntryFactory, LexiconEntryFactory>();
        services.AddTransient<IPostFactory, PostFactory>();
        services.AddTransient<IStudyGroupFactory, StudyGroupFactory>();

        return services;
    }
}