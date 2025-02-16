using LangApp.Core.Repositories;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Options;
using LangApp.Infrastructure.EF.Repositories.Exercises;
using LangApp.Infrastructure.EF.Repositories.Lexicons;
using LangApp.Infrastructure.EF.Repositories.Posts;
using LangApp.Infrastructure.EF.Repositories.StudyGroups;
using LangApp.Infrastructure.EF.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;

namespace LangApp.Infrastructure.EF;

internal static class Extensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ILexiconRepository, PostgresLexiconRepository>();
        services.AddScoped<IApplicationUserRepository, PostgresApplicationUserRepository>();
        services.AddScoped<IExerciseRepository, PostgresExerciseRepository>();
        services.AddScoped<IPostRepository, PostgresPostRepository>();
        services.AddScoped<IStudyGroupRepository, PostgresStudyGroupRepository>();

        var postgres = configuration.GetOptions<PostgresOptions>("Postgres");
        services.AddDbContext<ReadDbContext>(opt => opt.UseNpgsql(postgres.ConnectionString));
        services.AddDbContext<WriteDbContext>(opt => opt.UseNpgsql(postgres.ConnectionString));

        return services;
    }
}