using LangApp.Core.Entities.Posts;
using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Posts;

public interface IPostFactory
{
    Post Create(Guid authorId, Guid groupId, PostType type, string title, PostContent content);
    Post Create(Guid authorId, Guid groupId, PostType type, string title, PostContent content, List<string> media);
}