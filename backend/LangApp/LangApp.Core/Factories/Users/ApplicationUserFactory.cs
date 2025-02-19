using LangApp.Core.Entities;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Users;

public class ApplicationUserFactory : IApplicationUserFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public ApplicationUserFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public ApplicationUser Create(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email) => new(_keyGenerator.NewKey(), username, fullName, pictureUrl, role, email);

    public ApplicationUser Create(Guid id, Username username, UserFullName fullName, string? pictureUrl,
        AppUserRole role,
        string email) => new(id, username, fullName, pictureUrl, role, email);
}