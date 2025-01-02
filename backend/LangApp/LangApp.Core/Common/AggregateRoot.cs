namespace LangApp.Core.Common;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    protected void AddEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    protected void ClearEvents()
    {
        _domainEvents.Clear();
    }
}