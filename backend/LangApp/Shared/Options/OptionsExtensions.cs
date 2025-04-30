using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Options;

public static class OptionsExtensions
{
    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        var options = new TOptions();

        configuration.GetSection(sectionName).Bind(options);
        return options;
    }

    public static IServiceCollection AddOptions<TOptions>(this IServiceCollection services,
        IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        return services.Configure<TOptions>(configuration.GetSection(sectionName));
    }
}
