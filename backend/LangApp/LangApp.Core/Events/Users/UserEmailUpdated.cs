using LangApp.Core.Common;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Events.Users;

public record UserEmailUpdated(Email Email) : IDomainEvent;