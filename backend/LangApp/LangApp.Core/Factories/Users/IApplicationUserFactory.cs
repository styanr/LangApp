using LangApp.Core.Entities;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Users;

public interface IApplicationUserFactory
{
    ApplicationUser Create(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email);
}