using LangApp.Core.Common;
using LangApp.Core.Entities.Feeds;

namespace LangApp.Core.Events.Feeds;

public record FeedItemRemoved(Feed Feed, FeedItem Item) : IDomainEvent;