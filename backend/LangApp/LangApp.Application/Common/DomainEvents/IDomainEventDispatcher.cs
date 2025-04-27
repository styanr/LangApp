using LangApp.Core.Common;

namespace LangApp.Application.Common.DomainEvents;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> events);
}
