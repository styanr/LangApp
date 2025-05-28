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
        services.AddIdentity<IdentityApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Require email confirmation before sign in
            options.SignIn.RequireConfirmedEmail = true;
            
            // Configure password requirements
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            
            // Configure lockout
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
        })
            .AddEntityFrameworkStores<WriteDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.Section)
        );

        services.Configure<DataProtectionTokenProviderOptions>(options =>
            options.TokenLifespan = TimeSpan.FromHours(2));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenFactory, TokenFactory>();

        return services;
    }
}
