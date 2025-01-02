using LangApp.Core.Common;
using LangApp.Core.Entities.Feed;

namespace LangApp.Core.Events.FeedCtx;

public record FeedItemRemoved(Feed Feed, FeedItem Item) : IDomainEvent;