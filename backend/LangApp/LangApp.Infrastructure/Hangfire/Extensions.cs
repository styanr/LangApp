using Hangfire;
using LangApp.Application.Common.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure.Hangfire;

internal static class Extensions
{
    public static IServiceCollection AddHangfire(this IServiceCollection services
    )
    {
        // Configuring Hangfire to use in-memory storage
        services.AddHangfire(config => config.UseInMemoryStorage());

        // Adding the Hangfire server
        services.AddHangfireServer();

        services.AddSingleton<IJobScheduler, JobScheduler>();

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        return app.UseHangfireDashboard("/jobs");
    }
}
