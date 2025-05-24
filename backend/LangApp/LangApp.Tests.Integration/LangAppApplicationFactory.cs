using LangApp.Application.Common.Jobs;
using LangApp.Infrastructure.EF.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace LangApp.Tests.Integration;

public class LangAppApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithDatabase("test_db")
            .WithUsername("test")
            .WithPassword("test")
            .Build();


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Remove(services.Single(a => a.ServiceType == typeof(DbContextOptions<WriteDbContext>)));
            services.Remove(services.Single(a => a.ServiceType == typeof(DbContextOptions<ReadDbContext>)));

            services.AddDbContext<WriteDbContext>(options =>
                options.UseNpgsql(_postgresContainer.GetConnectionString()), ServiceLifetime.Scoped);

            services.AddDbContext<ReadDbContext>(options =>
                options.UseNpgsql(_postgresContainer.GetConnectionString()), ServiceLifetime.Scoped);

            var mockScheduler = new Mock<IJobScheduler>();
            services.RemoveAll<IJobScheduler>();
            services.AddSingleton(mockScheduler.Object);
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
}