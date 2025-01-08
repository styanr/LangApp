using System.Reflection;
using LangApp.Application.Common;
using LangApp.Application.Common.Abstractions;
using LangApp.Core.Factories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetCallingAssembly();

        services.AddSingleton<ICommandDispatcher, InMemoryCommandDispatcher>();
        services.Scan(s => s.FromAssemblies(assembly)
            .AddClasses(c => c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<IApplicationUserFactory, ApplicationUserFactory>();

        return services;
    }
}