using LangApp.Core.ValueObjects;

namespace LangApp.Core.Entities.Feeds;

public class FeedQuestion : FeedPost
{
    public FeedQuestion(Guid authorId, string title, PostContent content) : base(authorId, title, content)
    {
    }

    public FeedQuestion(Guid authorId, string title, PostContent content, List<string> media)
        : base(authorId, title, content, media)
    {
    }
}