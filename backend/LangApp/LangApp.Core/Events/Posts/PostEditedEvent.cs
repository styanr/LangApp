using LangApp.Core.Common;
using LangApp.Core.Entities.Posts;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.Posts;

public record PostEditedEvent(Post Post, PostContent Content) : IDomainEvent;