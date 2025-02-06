using System.Reflection;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Core.Factories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common.Queries;

public static class Extensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IApplicationUserFactory, ApplicationUserFactory>();

        return services;
    }
}