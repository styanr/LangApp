using LangApp.Core.Common;
using LangApp.Core.Entities.Feed;

namespace LangApp.Core.Events.FeedCtx;

public record FeedItemAdded(Feed Feed, FeedItem Item) : IDomainEvent;