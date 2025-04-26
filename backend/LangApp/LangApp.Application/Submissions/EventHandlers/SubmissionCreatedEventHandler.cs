using LangApp.Application.Common.Events.Handlers;
using LangApp.Core.Events.Submissions;
using Microsoft.Extensions.Logging;

namespace LangApp.Application.Submissions.EventHandlers;

public class SubmissionCreatedEventHandler : IDomainEventHandler<SubmissionCreated>
{
    private readonly ILogger<SubmissionCreatedEventHandler> _logger;

    public SubmissionCreatedEventHandler(ILogger<SubmissionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SubmissionCreated notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event Received: {}", notification);
        return Task.CompletedTask;
    }
}
