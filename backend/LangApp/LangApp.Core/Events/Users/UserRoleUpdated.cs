using LangApp.Core.Common;
using LangApp.Core.Enums;

namespace LangApp.Core.Events.Users;

public record UserRoleUpdated(AppUserRole Role) : IDomainEvent;