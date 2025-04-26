using LangApp.Application.Common.Events;
using LangApp.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LangApp.Infrastructure.EF.Interceptors;

public class EventPublishingInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;

    public EventPublishingInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is null) return await base.SavedChangesAsync(eventData, result, cancellationToken);

        var events = ExtractDomainEvents(eventData.Context);
        await _dispatcher.DispatchEventsAsync(events);

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private IEnumerable<IDomainEvent> ExtractDomainEvents(DbContext context)
    {
        var entities = context.ChangeTracker.Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity).ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();

        entities.ForEach(e => e.ClearEvents());
        return events;
    }
}
