using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public Username Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserFullName FullName { get; private set; }
    public string? PictureUrl { get; private set; }
    public AppUserRole Role { get; private set; }

    private void _init(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email,
        string passwordHash)
    {
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
        PasswordHash = passwordHash;
    }

    internal ApplicationUser(Guid id, Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email,
        string passwordHash) : base(id)
    {
        _init(username, fullName, pictureUrl, role, email, passwordHash);
    }

    internal ApplicationUser(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email,
        string passwordHash)
    {
        _init(username, fullName, pictureUrl, role, email, passwordHash);
    }
}