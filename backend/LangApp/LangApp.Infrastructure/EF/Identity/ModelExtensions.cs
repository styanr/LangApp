using LangApp.Core.Entities.Users;
using LangApp.Core.Factories.Users;
using LangApp.Core.ValueObjects;

namespace LangApp.Infrastructure.EF.Identity;

public static class ModelExtensions
{
    public static ApplicationUser ToDomainModel(this IdentityApplicationUser identityUser,
        IApplicationUserFactory factory)
    {
        var username = new Username(identityUser.UserName!);

        // TODO: emails
        var domainUser = factory.Create(identityUser.Id, username, identityUser.FullName,
            identityUser.PictureUrl,
            identityUser.Role, identityUser.Email!);

        return domainUser;
    }

    public static IdentityApplicationUser ToIdentityModel(this ApplicationUser domainModel)
    {
        return new IdentityApplicationUser(
            domainModel.Id,
            domainModel.Username,
            domainModel.FullName,
            domainModel.Email,
            domainModel.PictureUrl,
            domainModel.Role
        );
    }
}