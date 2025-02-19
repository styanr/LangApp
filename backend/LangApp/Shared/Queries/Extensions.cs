using System.Reflection;
using LangApp.Application.Common.Queries.Abstractions;
using LangApp.Core.Factories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Queries;

public static class Extensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IApplicationUserFactory, ApplicationUserFactory>();

        return services;
    }
}