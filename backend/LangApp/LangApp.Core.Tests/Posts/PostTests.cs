using System;
using System.Collections.Generic;
using FluentAssertions;
using LangApp.Core.Entities.Posts;
using LangApp.Core.Enums;
using LangApp.Core.Exceptions.Posts;
using LangApp.Core.ValueObjects;
using Xunit;

namespace LangApp.Core.Tests.Posts;

public class PostTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreatePostWithExpectedProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var authorId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var type = PostType.Discussion;
        var title = "Test Post";
        var content = new PostContent(new string('a', 50));

        // Act
        var post = new Post(id, authorId, groupId, type, title, content);

        // Assert
        post.Id.Should().Be(id);
        post.AuthorId.Should().Be(authorId);
        post.GroupId.Should().Be(groupId);
        post.Type.Should().Be(type);
        post.Title.Should().Be(title);
        post.Content.Should().Be(content);
        post.Archived.Should().BeFalse();
        post.Media.Should().BeEmpty();
        post.Comments.Should().BeEmpty();
    }

    [Fact]
    public void Edit_ShouldUpdateContentAndMediaAndSetEditedAt()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Resource, "Title", new PostContent(new string('b', 50)));
        var newContent = new PostContent(new string('c', 60));
        var media = new List<string> { "img1.png", "img2.png" };
        var oldEditedAt = post.EditedAt;

        // Act
        post.Edit(newContent, media);

        // Assert
        post.Content.Should().Be(newContent);
        post.Media.Should().BeEquivalentTo(media);
        post.EditedAt.Should().BeAfter(oldEditedAt);
        post.IsEdited.Should().BeTrue();
    }

    [Fact]
    public void Archive_ShouldSetArchivedTrue()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('d', 50)));

        // Act
        post.Archive();

        // Assert
        post.Archived.Should().BeTrue();
    }

    [Fact]
    public void Unarchive_ShouldSetArchivedFalse()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('e', 50)));
        post.Archive();

        // Act
        post.Unarchive();

        // Assert
        post.Archived.Should().BeFalse();
    }

    [Fact]
    public void CanBeModifiedBy_ShouldReturnTrueForAuthor()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var post = new Post(Guid.NewGuid(), authorId, Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('f', 50)));

        // Act
        var result = post.CanBeModifiedBy(authorId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanBeModifiedBy_ShouldReturnFalseForNonAuthor()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('g', 50)));
        var nonAuthorId = Guid.NewGuid();

        // Act
        var result = post.CanBeModifiedBy(nonAuthorId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddComment_ShouldAddCommentToPost()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('h', 50)));
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), post.Id, "Nice post!");

        // Act
        post.AddComment(comment);

        // Assert
        post.Comments.Should().ContainSingle().Which.Should().Be(comment);
    }

    [Fact]
    public void RemoveComment_ShouldRemoveCommentFromPost()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('i', 50)));
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), post.Id, "To be removed");
        post.AddComment(comment);

        // Act
        post.RemoveComment(comment.Id);

        // Assert
        post.Comments.Should().BeEmpty();
    }

    [Fact]
    public void RemoveComment_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('j', 50)));
        var invalidId = Guid.NewGuid();

        // Act
        Action act = () => post.RemoveComment(invalidId);

        // Assert
        act.Should().Throw<CommentNotFoundException>().WithMessage($"Comment with id {invalidId} not found");
    }

    [Fact]
    public void UpdateComment_ShouldUpdateCommentContent()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('k', 50)));
        var comment = PostComment.Create(Guid.NewGuid(), Guid.NewGuid(), post.Id, "Old content");
        post.AddComment(comment);
        var newContent = "Updated content";

        // Act
        post.UpdateComment(comment.Id, newContent);

        // Assert
        post.Comments.Should().ContainSingle();
        post.Comments.Should().ContainSingle(c => c.Content == newContent);
    }

    [Fact]
    public void UpdateComment_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var post = new Post(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PostType.Discussion, "Title", new PostContent(new string('l', 50)));
        var invalidId = Guid.NewGuid();

        // Act
        Action act = () => post.UpdateComment(invalidId, "content");

        // Assert
        act.Should().Throw<CommentNotFoundException>().WithMessage($"Comment with id {invalidId} not found");
    }
} 