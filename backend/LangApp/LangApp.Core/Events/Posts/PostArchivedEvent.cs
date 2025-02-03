using LangApp.Core.Common;
using LangApp.Core.Entities.Posts;

namespace LangApp.Core.Events.Posts;

public record PostArchivedEvent(Post Post) : IDomainEvent;