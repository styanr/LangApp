using LangApp.Infrastructure.EF;
using LangApp.Infrastructure.EF.Identity;
using LangApp.Infrastructure.EF.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Queries;

namespace LangApp.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddQueries();
        services.AddIdentityServices();

        services.AddHostedService<AppInitializer>();

        return services;
    }
}