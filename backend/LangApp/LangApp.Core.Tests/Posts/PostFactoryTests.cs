using System;
using System.Collections.Generic;
using FluentAssertions;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Enums;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;
using Moq;
using Xunit;

namespace LangApp.Core.Tests.Posts;

public class PostFactoryTests
{
    [Fact]
    public void Create_ShouldReturnPostWithExpectedProperties()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var type = PostType.Discussion;
        var title = "Factory Post";
        var content = new PostContent(new string('a', 50));
        var keyGenMock = new Mock<IKeyGenerator>();
        keyGenMock.Setup(x => x.NewKey()).Returns(expectedId);
        var factory = new PostFactory(keyGenMock.Object);

        // Act
        var post = factory.Create(authorId, groupId, type, title, content);

        // Assert
        post.Id.Should().Be(expectedId);
        post.AuthorId.Should().Be(authorId);
        post.GroupId.Should().Be(groupId);
        post.Type.Should().Be(type);
        post.Title.Should().Be(title);
        post.Content.Should().Be(content);
        post.Media.Should().BeEmpty();
    }

    [Fact]
    public void Create_WithMedia_ShouldReturnPostWithMedia()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var type = PostType.Resource;
        var title = "Factory Post With Media";
        var content = new PostContent(new string('b', 50));
        var media = new List<string> { "file1.png", "file2.jpg" };
        var keyGenMock = new Mock<IKeyGenerator>();
        keyGenMock.Setup(x => x.NewKey()).Returns(expectedId);
        var factory = new PostFactory(keyGenMock.Object);

        // Act
        var post = factory.Create(authorId, groupId, type, title, content, media);

        // Assert
        post.Id.Should().Be(expectedId);
        post.AuthorId.Should().Be(authorId);
        post.GroupId.Should().Be(groupId);
        post.Type.Should().Be(type);
        post.Title.Should().Be(title);
        post.Content.Should().Be(content);
        post.Media.Should().BeEquivalentTo(media);
    }
} 