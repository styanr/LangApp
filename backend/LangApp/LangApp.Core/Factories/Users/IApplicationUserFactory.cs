using LangApp.Core.Entities;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Users;

public interface IApplicationUserFactory
{
    ApplicationUser Create(Username username, UserFullName fullName, string? pictureUrl, UserRole role,
        Email email);

    ApplicationUser Create(Guid id, Username username, UserFullName fullName, string? pictureUrl, UserRole role,
        Email email);
}