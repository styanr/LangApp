using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserFullName FullName { get; private set; }
    public string? PictureUrl { get; private set; }
    public AppUserRole Role { get; private set; }

    public ApplicationUser(string username, UserFullName fullName, string? pictureUrl, AppUserRole role, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ApplicationUserUsernameEmptyException();
        }
        
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
        PasswordHash = passwordHash;
    }
}