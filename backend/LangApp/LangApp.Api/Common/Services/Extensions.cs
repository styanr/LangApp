using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Api.Common.Services;

public static class Extensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IHtmlTemplateService, HtmlTemplateService>();
        return services;
    }
} 