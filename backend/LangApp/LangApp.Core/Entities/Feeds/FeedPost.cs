using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Feeds;

public class FeedPost : FeedItem
{
    private readonly List<string> _media = new();

    public Guid AuthorId { get; private set; }
    public PostContent Content { get; private set; }
    public IReadOnlyCollection<string> Media => _media.AsReadOnly();

    public FeedPost(Guid authorId, string title, PostContent content) : base(title)
    {
        AuthorId = authorId;
        Content = content;
    }

    public FeedPost(Guid authorId, string title, PostContent content, List<string> media) : this(authorId, title,
        content)
    {
        _media = media;
    }
}