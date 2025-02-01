using LangApp.Core.Common;

namespace LangApp.Core.Events.Users;

public record UserPictureUrlUpdated(string? PictureUrl) : IDomainEvent;