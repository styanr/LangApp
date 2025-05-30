using LangApp.Core.Common;
using MediatR;

namespace LangApp.Application.Common.DomainEvents;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IMediator publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchEventsAsync(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            await _publisher.Publish(@event);
        }
    }
}