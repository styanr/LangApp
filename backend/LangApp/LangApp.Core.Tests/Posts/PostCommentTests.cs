using System;
using FluentAssertions;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Exceptions.Posts;
using Xunit;

namespace LangApp.Core.Tests.Posts;

public class PostCommentTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateCommentWithExpectedProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var content = "This is a comment.";

        // Act
        var comment = PostComment.Create(id, authorId, postId, content);

        // Assert
        comment.Id.Should().Be(id);
        comment.AuthorId.Should().Be(authorId);
        comment.PostId.Should().Be(postId);
        comment.Content.Should().Be(content);
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        comment.EditedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyContent_ShouldThrowArgumentException(string invalidContent)
    {
        // Arrange
        var id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        // Act
        Action act = () => PostComment.Create(id, authorId, postId, invalidContent);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Comment content cannot be empty*");
    }

    [Fact]
    public void Create_WithTooLongContent_ShouldThrowPostCommentLengthException()
    {
        // Arrange
        var id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var postId = Guid.NewGuid();
        var tooLongContent = new string('a', 101);

        // Act
        Action act = () => PostComment.Create(id, authorId, postId, tooLongContent);

        // Assert
        act.Should().Throw<PostCommentLengthException>().Where(e => e.ContentLength == 101 && e.MaxLength == 100);
    }

    [Fact]
    public void UpdateContent_ShouldChangeContentAndSetEditedAt()
    {
        // Arrange
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Original content");
        var newContent = "Updated content";

        // Act
        comment.UpdateContent(newContent);

        // Assert
        comment.Content.Should().Be(newContent);
        comment.EditedAt.Should().NotBeNull();
        comment.EditedAt.Should().BeAfter(comment.CreatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateContent_WithEmptyContent_ShouldThrowArgumentException(string invalidContent)
    {
        // Arrange
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Original content");

        // Act
        Action act = () => comment.UpdateContent(invalidContent);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Comment content cannot be empty*");
    }

    [Fact]
    public void IsAuthor_ShouldReturnTrueForAuthor()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var comment = PostComment.Create(Guid.NewGuid(), authorId, Guid.NewGuid(), "content");

        // Act
        var result = comment.IsAuthor(authorId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAuthor_ShouldReturnFalseForNonAuthor()
    {
        // Arrange
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "content");
        var nonAuthorId = Guid.NewGuid();

        // Act
        var result = comment.IsAuthor(nonAuthorId);

        // Assert
        result.Should().BeFalse();
    }
} 