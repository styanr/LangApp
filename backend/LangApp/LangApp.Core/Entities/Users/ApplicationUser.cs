using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Events.Users;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Users;

public class ApplicationUser : AggregateRoot
{
    public Username Username { get; private set; }
    public string Email { get; private set; }
    public UserFullName FullName { get; private set; }
    public string? PictureUrl { get; private set; }
    public AppUserRole Role { get; private set; }

    private void Init(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email)
    {
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
    }

    private ApplicationUser()
    {
    }

    internal ApplicationUser(Guid id, Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email) : base(id)
    {
        Init(username, fullName, pictureUrl, role, email);
    }

    internal ApplicationUser(Username username, UserFullName fullName, string? pictureUrl, AppUserRole role,
        string email)
    {
        Init(username, fullName, pictureUrl, role, email);
    }

    public void UpdateUsername(Username username)
    {
        if (Username == username) return;

        Username = username;
        AddEvent(new UserUsernameUpdated(username));
    }

    public void UpdateFullName(UserFullName fullName)
    {
        if (FullName == fullName) return;

        FullName = fullName;

        AddEvent(new UserFullNameUpdated(FullName));
    }

    public void UpdatePictureUrl(string? pictureUrl)
    {
        if (PictureUrl == pictureUrl) return;

        PictureUrl = pictureUrl;

        AddEvent(new UserPictureUrlUpdated(pictureUrl));
    }

    public void UpdateRole(AppUserRole role)
    {
        if (Role == role) return;

        Role = role;

        AddEvent(new UserRoleUpdated(role));
    }
}