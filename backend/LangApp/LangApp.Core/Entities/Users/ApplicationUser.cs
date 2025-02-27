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
    public UserRole Role { get; private set; }

    private ApplicationUser()
    {
    }

    internal ApplicationUser(Guid id, Username username, UserFullName fullName, string? pictureUrl, UserRole role,
        string email) : base(id)
    {
        Username = username;
        FullName = fullName;
        PictureUrl = pictureUrl;
        Role = role;
        Email = email;
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

    public void UpdateRole(UserRole role)
    {
        if (Role == role) return;

        Role = role;

        AddEvent(new UserRoleUpdated(role));
    }
}