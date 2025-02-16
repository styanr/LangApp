using LangApp.Infrastructure.EF.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure.EF.Identity;

public static class Extensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<IdentityApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<WriteDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }
}