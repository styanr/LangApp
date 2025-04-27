using LangApp.Infrastructure.EF;
using LangApp.Infrastructure.EF.Identity;
using LangApp.Infrastructure.EF.Services;
using LangApp.Infrastructure.Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Queries;

namespace LangApp.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPostgres(configuration);
        services.AddHangfire();
        services.AddQueries();
        services.AddIdentityServices(configuration);

        services.AddHostedService<AppInitializer>();

        return services;
    }


    public static IApplicationBuilder UseInfrastructureEndpoints(this IApplicationBuilder app)
    {
        return app.UseHangfire();
    }
}
