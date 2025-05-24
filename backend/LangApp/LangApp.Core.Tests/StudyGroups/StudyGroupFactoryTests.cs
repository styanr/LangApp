using System;
using FluentAssertions;
using LangApp.Core.Entities.StudyGroups;
using LangApp.Core.Factories.StudyGroups;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;
using Moq;
using Xunit;

namespace LangApp.Core.Tests.StudyGroups;

public class StudyGroupFactoryTests
{
    [Fact]
    public void Create_ShouldReturnGroupWithExpectedProperties()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var name = "Test Group";
        var language = Language.EnglishUS;
        var ownerId = Guid.NewGuid();
        var keyGenMock = new Mock<IKeyGenerator>();
        keyGenMock.Setup(x => x.NewKey()).Returns(expectedId);
        var factory = new StudyGroupFactory(keyGenMock.Object);

        // Act
        var group = factory.Create(name, language, ownerId);

        // Assert
        group.Id.Should().Be(expectedId);
        group.Name.Should().Be(name);
        group.Language.Should().Be(language);
        group.OwnerId.Should().Be(ownerId);
        group.Members.Should().BeEmpty();
    }
} 