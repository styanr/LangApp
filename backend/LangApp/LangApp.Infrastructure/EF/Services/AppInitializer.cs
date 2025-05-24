using LangApp.Infrastructure.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LangApp.Infrastructure.EF.Services;

internal sealed class AppInitializer : IHostedService
{
    private readonly IServiceProvider _provider;

    public AppInitializer(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _provider.CreateScope();

        // TODO a more flexible approach?
        DbContext[] dbContextList =
        [
            scope.ServiceProvider.GetRequiredService<ReadDbContext>(),
            scope.ServiceProvider.GetRequiredService<WriteDbContext>()
        ];

        var dbContext = dbContextList[1];
        // Console.WriteLine("!!!!!!!!!!!!!! MIGRATING " + dbContext.GetType().Name);
        // // list all migration files in the current directory
        // var files = Directory.GetFiles(
        //     Path.Combine(Directory.GetParent(Environment.CurrentDirectory).FullName, "LangApp.Infrastructure", "EF",
        //         "Migrations"),
        //     "*.cs",
        //     SearchOption.AllDirectories);
        //
        // foreach (var file in files)
        // {
        //     Console.WriteLine($"Migration file: {file}");
        // }
        //
        // var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        // if (pendingMigrations.Any())
        // {
        //     Console.WriteLine($"Pending migrations for {dbContext.GetType().Name}:");
        //     foreach (var migration in pendingMigrations)
        //     {
        //         Console.WriteLine($"- {migration}");
        //     }
        // }
        // else
        // {
        //     Console.WriteLine($"No pending migrations for {dbContext.GetType().Name}.");
        // }
        //
        // var allMigrations = dbContext.Database.GetMigrations();
        // if (allMigrations.Any())
        // {
        //     Console.WriteLine($"All migrations for {dbContext.GetType().Name}:");
        //     foreach (var migration in allMigrations)
        //     {
        //         Console.WriteLine($"- {migration}");
        //     }
        // }
        // else
        // {
        //     Console.WriteLine($"No migrations found for {dbContext.GetType().Name}.");
        // }
        //
        // var appliedMigrations = dbContext.Database.GetAppliedMigrations();
        // if (appliedMigrations.Any())
        // {
        //     Console.WriteLine($"Already applied migrations for {dbContext.GetType().Name}:");
        //     foreach (var migration in appliedMigrations)
        //     {
        //         Console.WriteLine($"- {migration}");
        //     }
        // }
        // else
        // {
        //     Console.WriteLine($"No migrations have been applied for {dbContext.GetType().Name}.");
        // }

        await dbContext.Database.MigrateAsync(cancellationToken);

        // var migrations = dbContext.Database.GetAppliedMigrations().ToList();
        // if (migrations.Any())
        // {
        //     Console.WriteLine($"Applied migrations for {dbContext.GetType().Name}:");
        //     foreach (var migration in migrations)
        //     {
        //         Console.WriteLine($"- {migration}");
        //     }
        // }
        // else
        // {
        //     Console.WriteLine($"No migrations applied for {dbContext.GetType().Name}.");
        // }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}