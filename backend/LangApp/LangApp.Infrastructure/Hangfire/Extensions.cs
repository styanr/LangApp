using Hangfire;
using Hangfire.PostgreSql;
using LangApp.Application.Common.Jobs;
using LangApp.Infrastructure.Hangfire.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Infrastructure.Hangfire;

internal static class Extensions
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfire = configuration.GetOptions<HangfireOptions>("Hangfire");
        services.AddHangfire(config =>
        {
            config.UseFilter(new AutomaticRetryAttribute { Attempts = 3 });
            config.UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(hangfire.ConnectionString));
        });

        services.AddHangfireServer();

        services.AddSingleton<IJobScheduler, JobScheduler>();

        return services;
    }

    public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
    {
        return app.UseHangfireDashboard("/jobs");
    }
}