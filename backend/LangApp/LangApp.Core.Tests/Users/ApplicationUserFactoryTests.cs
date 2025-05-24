using System;
using FluentAssertions;
using LangApp.Core.Entities.Users;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Users;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;
using Moq;
using Xunit;

namespace LangApp.Core.Tests.Users;

public class ApplicationUserFactoryTests
{
    [Fact]
    public void Create_ShouldReturnUserWithExpectedProperties()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var username = new Username("user1");
        var fullName = new UserFullName("First", "Last");
        var pictureUrl = "http://pic.url";
        var role = UserRole.Teacher;
        var email = "user1@email.com";
        var keyGenMock = new Mock<IKeyGenerator>();
        keyGenMock.Setup(x => x.NewKey()).Returns(expectedId);
        var factory = new ApplicationUserFactory(keyGenMock.Object);

        // Act
        var user = factory.Create(username, fullName, pictureUrl, role, email);

        // Assert
        user.Id.Should().Be(expectedId);
        user.Username.Should().Be(username);
        user.FullName.Should().Be(fullName);
        user.PictureUrl.Should().Be(pictureUrl);
        user.Role.Should().Be(role);
        user.Email.Should().Be(email);
    }

    [Fact]
    public void Create_WithExplicitId_ShouldReturnUserWithThatId()
    {
        // Arrange
        var explicitId = Guid.NewGuid();
        var username = new Username("user2");
        var fullName = new UserFullName("Jane", "Doe");
        var pictureUrl = "http://pic2.url";
        var role = UserRole.Student;
        var email = "user2@email.com";
        var keyGenMock = new Mock<IKeyGenerator>();
        var factory = new ApplicationUserFactory(keyGenMock.Object);

        // Act
        var user = factory.Create(explicitId, username, fullName, pictureUrl, role, email);

        // Assert
        user.Id.Should().Be(explicitId);
        user.Username.Should().Be(username);
        user.FullName.Should().Be(fullName);
        user.PictureUrl.Should().Be(pictureUrl);
        user.Role.Should().Be(role);
        user.Email.Should().Be(email);
    }
} 