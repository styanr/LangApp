using LangApp.Application.Users.Services;
using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Interceptors;
using LangApp.Infrastructure.EF.Options;
using LangApp.Infrastructure.EF.Repositories.Assignments;
using LangApp.Infrastructure.EF.Repositories.Lexicons;
using LangApp.Infrastructure.EF.Repositories.Posts;
using LangApp.Infrastructure.EF.Repositories.StudyGroups;
using LangApp.Infrastructure.EF.Repositories.Submissions;
using LangApp.Infrastructure.EF.Repositories.Users;
using LangApp.Infrastructure.EF.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Infrastructure.EF;

internal static class Extensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ILexiconRepository, PostgresLexiconRepository>();
        services.AddScoped<IApplicationUserRepository, IdentityUserRepository>();
        services.AddScoped<IAssignmentRepository, PostgresAssignmentRepository>();
        services.AddScoped<IPostRepository, PostgresPostRepository>();
        services.AddScoped<IStudyGroupRepository, PostgresStudyGroupRepository>();
        services.AddScoped<ISubmissionRepository, PostgresSubmissionRepository>();

        services.AddScoped<IApplicationUserReadService, ApplicationUserReadService>();

        services.AddScoped<EventPublishingInterceptor>();

        var postgres = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<ReadDbContext>(opt =>
            opt.UseNpgsql(postgres.ConnectionString)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddDbContext<WriteDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(postgres.ConnectionString);
            opt.AddInterceptors(sp.GetRequiredService<EventPublishingInterceptor>());
            opt.EnableSensitiveDataLogging();
        });

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

        return services;
    }
}
