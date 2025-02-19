using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Core.Events.Posts;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Posts;

public class Post : AggregateRoot
{
    private readonly List<string> _media = [];

    public PostType Type { get; set; }
    public string Title { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid GroupId { get; private set; }
    public bool Archived { get; private set; } = true;
    public PostContent Content { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public DateTime EditedAt { get; private set; } = DateTime.Now;
    public IReadOnlyCollection<string> Media => _media.AsReadOnly();

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

    public void Edit(PostContent content)
    {
        Content = content;
        EditedAt = DateTime.Now;

        AddEvent(new PostEditedEvent(this, content));
    }

    public void Archive()
    {
        Archived = true;

        AddEvent(new PostArchivedEvent(this));
    }
}