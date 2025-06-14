using LangApp.Application.Common.Jobs;
using LangApp.Application.Common.Services;
using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Infrastructure.BlobStorage;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.PronunciationAssessment;
using LangApp.Infrastructure.PronunciationAssessment.Audio;
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


            var mockEmailService = new Mock<IEmailService>();
            services.RemoveAll<IEmailService>();
            services.AddSingleton(mockEmailService.Object);


            var mockRecordingStorageService = new Mock<IRecordingStorageService>();
            services.RemoveAll<IRecordingStorageService>();
            services.AddSingleton(mockRecordingStorageService.Object);

            var mockPronunciationAssessmentService = new Mock<IPronunciationAssessmentService>();
            services.RemoveAll<IPronunciationAssessmentService>();
            services.AddSingleton(mockPronunciationAssessmentService.Object);

            var mockAudioFetcher = new Mock<IAudioFetcher>();
            services.RemoveAll<IAudioFetcher>();
            services.AddSingleton(mockAudioFetcher.Object);
            
            Console.WriteLine(_postgresContainer.GetConnectionString());
            
            services.AddDbContext<WriteDbContext>(options =>
                options.UseNpgsql(_postgresContainer.GetConnectionString()), ServiceLifetime.Scoped);

            services.AddDbContext<ReadDbContext>(options =>
                options.UseNpgsql(_postgresContainer.GetConnectionString()), ServiceLifetime.Scoped);

            var mockScheduler = new Mock<IJobScheduler>();
            services.RemoveAll<IJobScheduler>();
            services.AddSingleton(mockScheduler.Object);


            using var scope = services.BuildServiceProvider().CreateScope();

            // Ensure the database is created
            // using var scope = services.BuildServiceProvider().CreateScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
            // dbContext.Database.EnsureDeleted();
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