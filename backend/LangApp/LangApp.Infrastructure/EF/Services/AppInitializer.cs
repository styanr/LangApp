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

        // Use WriteDbContext for migrations since it contains the full schema
        var writeDbContext = scope.ServiceProvider.GetRequiredService<WriteDbContext>();

        await writeDbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}