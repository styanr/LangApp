using LangApp.Core.Services.PronunciationAssessment;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Infrastructure.PronunciationAssessment;

public static class Extensions
{
    public static IServiceCollection AddPronunctiationAssessment(this IServiceCollection services)
    {
        services.AddScoped<IPronunciationAssessmentService, PronunciationAssessmentService>(); // TODO TEMP

        return services;
    }
}
