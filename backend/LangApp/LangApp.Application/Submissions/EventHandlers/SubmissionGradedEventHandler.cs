using LangApp.Application.Common.DomainEvents.Handlers;
using LangApp.Application.Common.Jobs;
using LangApp.Application.Submissions.Jobs;
using LangApp.Core.Events.Submissions;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.EventHandlers;

public class SubmissionGradedEventHandler : IDomainEventHandler<SubmissionGraded>
{
    private readonly ILogger<SubmissionCreatedEventHandler> _logger;
    private readonly IJobScheduler _scheduler;

    public SubmissionGradedEventHandler(ILogger<SubmissionCreatedEventHandler> logger, IJobScheduler scheduler)
    {
        _logger = logger;
        _scheduler = scheduler;
    }

    public Task Handle(SubmissionGraded notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SubmissionGraded, SubmissionId: {SubmissionId}", notification.Submission.Id);

        _scheduler.Enqueue<NotifyUserSubmissionGradedJob, NotifyUserSubmissionGradedJobData>(job =>
            job.ExecuteAsync(new(
                notification.Submission.StudentId,
                notification.Submission.Id,
                notification.Submission.Score)
            ));

        return Task.CompletedTask;
    }
}