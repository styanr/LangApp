using LangApp.Core.Common;
using LangApp.Core.Events.Feeds;
using LangApp.Core.Exceptions.Feeds;

namespace LangApp.Core.Entities.Feeds;

public class Feed : AggregateRoot
{
    private readonly List<FeedItem> _items = new();

    public Guid GroupId { get; private set; }

    public IReadOnlyCollection<FeedItem> Items => _items;

    public Feed(Guid groupId)
    {
        GroupId = groupId;
    }

    public void AddItem(FeedItem item)
    {
        if (_items.Any(i => i.Id == item.Id))
        {
            throw new ItemAlreadyExistsException(item.Id);
        }

        _items.Add(item);

        AddEvent(new FeedItemAdded(this, item));
    }

    public void RemoveItem(FeedItem item)
    {
        if (_items.Any(i => i.Id != item.Id))
        {
            throw new ItemNotFoundException(item.Id);
        }

        _items.Remove(item);

        AddEvent(new FeedItemRemoved(this, item));
    }
}