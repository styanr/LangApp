using LangApp.Core.Common;
using System;
using LangApp.Core.Exceptions.Posts;

namespace LangApp.Core.Entities.Posts;

public class PostComment : BaseEntity
{
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? EditedAt { get; private set; }
    public Guid PostId { get; private set; }

    private PostComment()
    {
    }

    public static PostComment Create(Guid id, Guid authorId, Guid postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Comment content cannot be empty", nameof(content));

        if (content.Length > 100)
            throw new PostCommentLengthException(content.Length, 100);

        return new PostComment
        {
            Id = id,
            PostId = postId,
            Content = content,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Comment content cannot be empty", nameof(newContent));

        Content = newContent;
        EditedAt = DateTime.UtcNow;
    }

    public bool IsAuthor(Guid userId)
    {
        return AuthorId == userId;
    }
}