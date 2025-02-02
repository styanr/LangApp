using LangApp.Core.Entities;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Users;

public class ApplicationUserFactory : IApplicationUserFactory
{
    public ApplicationUser Create(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email) => new(username, fullName, pictureUrl, role, email);
}