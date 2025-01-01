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

    private ApplicationUser(Guid id, DateTimeOffset dateCreated, DateTimeOffset dateUpdated, DateTimeOffset dateDeleted,
        bool isDeleted, Username username, UserFullName fullName, string? pictureUrl, AppUserRole role, string email,
        string passwordHash) : base(id, dateCreated, dateUpdated, dateDeleted, isDeleted)
    {
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
        PasswordHash = passwordHash;
    }

    internal ApplicationUser(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
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
}