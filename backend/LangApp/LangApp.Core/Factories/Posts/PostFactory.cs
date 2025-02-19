using LangApp.Core.Entities.Posts;
using LangApp.Core.Enums;
using LangApp.Core.Services.KeyGeneration;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Posts;

public class PostFactory : IPostFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public PostFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public Post Create(Guid authorId, Guid groupId, PostType type, string title, PostContent content)
    {
        return new Post(_keyGenerator.NewKey(), authorId, groupId, type, title, content);
    }

    public Post Create(Guid authorId, Guid groupId, PostType type, string title, PostContent content,
        List<string> media)
    {
        return new Post(_keyGenerator.NewKey(), authorId, groupId, type, title, content, media);
    }
}