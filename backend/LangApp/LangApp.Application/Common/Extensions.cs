using System.Reflection;
using LangApp.Application.Common.Commands;
using LangApp.Application.Common.Queries;
using LangApp.Core.Factories.Users;
using LangApp.Core.Services.EvaluationStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common;

public static class Extensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // services.AddCommands();
        // services.AddQueries();

        var assembly = Assembly.GetAssembly(typeof(IEvaluationStrategy<,>));

        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IEvaluationStrategy<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IApplicationUserFactory, ApplicationUserFactory>();

        return services;
    }
}