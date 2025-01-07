using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities;

public class ApplicationUser : BaseEntity
{
    public Username Username { get; private set; }
    public string Email { get; private set; }
    public UserFullName FullName { get; private set; }
    public string? PictureUrl { get; private set; }
    public AppUserRole Role { get; private set; }

    private void _init(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email)
    {
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
    }

    internal ApplicationUser(Guid id, Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email) : base(id)
    {
        _init(username, fullName, pictureUrl, role, email);
    }

    internal ApplicationUser(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email)
    {
        _init(username, fullName, pictureUrl, role, email);
    }
}