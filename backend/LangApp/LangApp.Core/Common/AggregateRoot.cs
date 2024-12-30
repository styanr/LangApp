namespace LangApp.Core.Common;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset dateDeleted,
        bool isDeleted) : base(
        id,
        dateCreated,
        dateUpdated,
        dateDeleted,
        isDeleted)
    {
    }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    public void CreateEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }
}