using LangApp.Core.Services.GradingStrategies;
using LangApp.Core.ValueObjects;
using LangApp.Core.ValueObjects.Assignments;
using LangApp.Core.ValueObjects.Submissions;
using Microsoft.Extensions.DependencyInjection;

namespace LangApp.Application.Common.Strategies;

public class InMemoryGradingStrategyDispatcher : IGradingStrategyDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryGradingStrategyDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<SubmissionGrade> Grade<TAssignmentDetails>(TAssignmentDetails assignmentDetails,
        SubmissionDetails submissionDetails, CancellationToken cancellationToken = default(CancellationToken))
        where TAssignmentDetails : ActivityDetails
    {
        using var scope = _serviceProvider.CreateScope();

        var handlerType = typeof(IGradingStrategy<>).MakeGenericType(assignmentDetails.GetType());
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IGradingStrategy<TAssignmentDetails>.Grade));

        return await (Task<SubmissionGrade>)method!.Invoke(handler,
            [assignmentDetails, submissionDetails, cancellationToken])!;
    }
}