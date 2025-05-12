using LangApp.Application.Common.Services;
using LangApp.Infrastructure.Email.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Infrastructure.Email;

public static class Extensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services,
        IConfiguration configuration)
    {
        var sendGrid = configuration.GetOptions<SendGridOptions>("SendGrid");
        services.AddOptions<SendGridOptions>(configuration, "SendGrid");

        services.AddSendGrid(options => { options.ApiKey = sendGrid.ApiKey; });

        services.AddScoped<IEmailService, SendGridEmailService>();

        return services;
    }
}
