using LangApp.Core.Common;

namespace LangApp.Core.Entities.Feed;

public class FeedItem : BaseEntity
{
    public string Title { get; private set; }

    public FeedItem(string title)
    {
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentNullException(nameof(title), "Expression cannot be null or empty.");
        }

        Title = title;
    }
}