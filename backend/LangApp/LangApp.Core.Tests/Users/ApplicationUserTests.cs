using System;
using FluentAssertions;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;
using Xunit;

namespace LangApp.Core.Tests.Users;

public class ApplicationUserTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var username = new Username("user1");
        var fullName = new UserFullName("First", "Last");
        var pictureUrl = "http://pic.url";
        var role = UserRole.Teacher;
        var email = "user1@email.com";

        // Act
        var user = (ApplicationUser)Activator.CreateInstance(typeof(ApplicationUser), true)!;
        user.GetType().GetProperty("Id")!.SetValue(user, id);
        user.GetType().GetProperty("Username")!.SetValue(user, username);
        user.GetType().GetProperty("FullName")!.SetValue(user, fullName);
        user.GetType().GetProperty("PictureUrl")!.SetValue(user, pictureUrl);
        user.GetType().GetProperty("Role")!.SetValue(user, role);
        user.GetType().GetProperty("Email")!.SetValue(user, email);

        // Assert
        user.Id.Should().Be(id);
        user.Username.Should().Be(username);
        user.FullName.Should().Be(fullName);
        user.PictureUrl.Should().Be(pictureUrl);
        user.Role.Should().Be(role);
        user.Email.Should().Be(email);
    }

    [Fact]
    public void UpdateUsername_ShouldChangeUsernameAndAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var newUsername = new Username("newuser");

        // Act
        user.UpdateUsername(newUsername);

        // Assert
        user.Username.Should().Be(newUsername);
        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "UserUsernameUpdated");
    }

    [Fact]
    public void UpdateUsername_WithSameUsername_ShouldNotAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var sameUsername = user.Username;

        // Act
        user.UpdateUsername(sameUsername);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdateFullName_ShouldChangeFullNameAndAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var newFullName = new UserFullName("New", "Name");

        // Act
        user.UpdateFullName(newFullName);

        // Assert
        user.FullName.Should().Be(newFullName);
        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "UserFullNameUpdated");
    }

    [Fact]
    public void UpdateFullName_WithSameFullName_ShouldNotAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var sameFullName = user.FullName;

        // Act
        user.UpdateFullName(sameFullName);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdatePictureUrl_ShouldChangePictureUrlAndAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var newUrl = "http://new.pic";

        // Act
        user.UpdatePictureUrl(newUrl);

        // Assert
        user.PictureUrl.Should().Be(newUrl);
        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "UserPictureUrlUpdated");
    }

    [Fact]
    public void UpdatePictureUrl_WithSameUrl_ShouldNotAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var sameUrl = user.PictureUrl;

        // Act
        user.UpdatePictureUrl(sameUrl);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void UpdateRole_ShouldChangeRoleAndAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var newRole = user.Role == UserRole.Teacher ? UserRole.Student : UserRole.Teacher;

        // Act
        user.UpdateRole(newRole);

        // Assert
        user.Role.Should().Be(newRole);
        user.DomainEvents.Should().ContainSingle(e => e.GetType().Name == "UserRoleUpdated");
    }

    [Fact]
    public void UpdateRole_WithSameRole_ShouldNotAddEvent()
    {
        // Arrange
        var user = CreateUser();
        var sameRole = user.Role;

        // Act
        user.UpdateRole(sameRole);

        // Assert
        user.DomainEvents.Should().BeEmpty();
    }

    private static ApplicationUser CreateUser()
    {
        var user = (ApplicationUser)Activator.CreateInstance(typeof(ApplicationUser), true)!;
        user.GetType().GetProperty("Id")!.SetValue(user, Guid.NewGuid());
        user.GetType().GetProperty("Username")!.SetValue(user, new Username("user1"));
        user.GetType().GetProperty("FullName")!.SetValue(user, new UserFullName("First", "Last"));
        user.GetType().GetProperty("PictureUrl")!.SetValue(user, "http://pic.url");
        user.GetType().GetProperty("Role")!.SetValue(user, UserRole.Teacher);
        user.GetType().GetProperty("Email")!.SetValue(user, "user1@email.com");
        return user;
    }
} 