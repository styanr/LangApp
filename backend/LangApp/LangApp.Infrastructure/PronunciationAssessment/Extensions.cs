using LangApp.Core.Services.PronunciationAssessment;
using LangApp.Infrastructure.PronunciationAssessment.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Options;

namespace LangApp.Infrastructure.PronunciationAssessment;

public static class Extensions
{
    public static IServiceCollection AddPronunctiationAssessment(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<SpeechConfigOptions>(configuration, "Azure:Speech");
        services.AddScoped<IPronunciationAssessmentService, PronunciationAssessmentService>(); // TODO TEMP

        return services;
    }
}
