using LangApp.Core.Common;

namespace LangApp.Application.Common.Events;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<IDomainEvent> events);
}
