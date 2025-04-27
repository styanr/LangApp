using LangApp.Application.Common.DomainEvents.Handlers;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Submissions.Jobs;
using LangApp.Core.Events.Submissions;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.EventHandlers;

public class SubmissionCreatedEventHandler : IDomainEventHandler<SubmissionCreated>
{
    private readonly ILogger<SubmissionCreatedEventHandler> _logger;
    private readonly IJobScheduler _scheduler;

    public SubmissionCreatedEventHandler(ILogger<SubmissionCreatedEventHandler> logger, IJobScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
    }

    public Task Handle(SubmissionCreated notification, CancellationToken cancellationToken)
    {
        _scheduler.Enqueue<SubmissionGradingJob>(job => job.Execute(new(notification.Submission.Id)));
        return Task.CompletedTask;
    }
}
