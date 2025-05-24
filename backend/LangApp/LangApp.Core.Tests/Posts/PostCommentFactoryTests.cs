using System;
using FluentAssertions;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Factories.Posts;
using LangApp.Core.Services.KeyGeneration;
using Moq;
using Xunit;

namespace LangApp.Core.Tests.Posts;

public class PostCommentFactoryTests
{
    [Fact]
    public void Create_ShouldReturnCommentWithExpectedProperties()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var content = "Factory comment content";
        var keyGenMock = new Mock<IKeyGenerator>();
        keyGenMock.Setup(x => x.NewKey()).Returns(expectedId);
        var factory = new PostCommentFactory(keyGenMock.Object);

        // Act
        var comment = factory.Create(authorId, postId, content);

        // Assert
        comment.Id.Should().Be(expectedId);
        comment.AuthorId.Should().Be(authorId);
        comment.PostId.Should().Be(postId);
        comment.Content.Should().Be(content);
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        comment.EditedAt.Should().BeNull();
    }
} 