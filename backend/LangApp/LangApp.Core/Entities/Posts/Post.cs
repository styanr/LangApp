using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Events.Posts;
using LangApp.Core.Exceptions.Posts;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Posts;

public class Post : AggregateRoot
{
    private readonly List<string> _media = [];
    private readonly List<PostComment> _comments = [];

    public PostType Type { get; set; }
    public string Title { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid GroupId { get; private set; }
    public bool Archived { get; private set; } = false;
    public PostContent Content { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime EditedAt { get; private set; } = DateTime.UtcNow;
    public bool IsEdited => CreatedAt != EditedAt;
    public IReadOnlyCollection<string> Media => _media.AsReadOnly();
    public IReadOnlyCollection<PostComment> Comments => _comments.AsReadOnly();

    private Post()
    {
    }

    internal Post(Guid id, Guid authorId, Guid groupId, PostType type, string title, PostContent content) : base(id)
    {
        Type = type;
        Title = title;
        AuthorId = authorId;
        GroupId = groupId;
        Content = content;
    }

    internal Post(Guid id, Guid authorId, Guid groupId, PostType type, string title, PostContent content,
        List<string> media) :
        this(id, authorId, groupId, type, title, content)
    {
        _media = media;
    }

    public void Edit(PostContent content, List<string> media)
    {
        Content = content;
        _media.Clear();
        _media.AddRange(media);

        UpdateEditedAt();

        AddEvent(new PostEditedEvent(this, content));
    }

    public void Archive()
    {
        Archived = true;

        AddEvent(new PostArchivedEvent(this));
    }

    public void Unarchive()
    {
        Archived = false;

        AddEvent(new PostUnarchivedEvent(this));
    }

    private void UpdateEditedAt()
    {
        EditedAt = DateTime.UtcNow;
    }

    public bool CanBeModifiedBy(Guid userId)
    {
        return AuthorId == userId;
    }

    public void AddComment(PostComment comment)
    {
        _comments.Add(comment);
    }

    public void RemoveComment(Guid commentId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            throw new CommentNotFoundException(commentId);
        }

        _comments.Remove(comment);
    }

    public void UpdateComment(Guid commentId, string content)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
        {
            throw new CommentNotFoundException(commentId);
        }

        comment.UpdateContent(content);
    }
}
