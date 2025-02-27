using LangApp.Application.Auth.Services;
using LangApp.Infrastructure.EF.Context;
using LangApp.Infrastructure.EF.Options;
using LangApp.Infrastructure.EF.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure.EF.Identity;

public static class Extensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<IdentityApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<WriteDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.Section)
        );

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenFactory, TokenFactory>();

        return services;
    }
}