using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace LangApp.Infrastructure.EF.Identity;

public sealed class IdentityApplicationUser : IdentityUser<Guid>
{
    public UserFullName FullName { get; set; }
    public string? PictureUrl { get; set; }
    public UserRole Role { get; set; }

    private IdentityApplicationUser()
    {
    }

    public IdentityApplicationUser(Guid id, Username username, UserFullName fullName, string email, string? pictureUrl,
        UserRole role)
    {
        Id = id;
        UserName = username.ToString();
        FullName = fullName;
        Email = email;
        PictureUrl = pictureUrl;
        Role = role;
    }
}