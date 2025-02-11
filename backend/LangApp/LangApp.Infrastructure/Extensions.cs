using System.Reflection;
using LangApp.Core.Factories.Users;
using LangApp.Core.Services.EvaluationStrategies;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}