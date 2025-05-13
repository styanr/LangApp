using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Application.Auth.Options;

public static class OptionsExtensions
{
    public static IServiceCollection AddDeeplinkConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<PublicOptions>(configuration, "Public");
        return services.AddOptions<ClientAppOptions>(configuration, "ClientApp");
    }
}
