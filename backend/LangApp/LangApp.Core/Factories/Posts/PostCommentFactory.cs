using LangApp.Core.Entities.Posts;
using LangApp.Core.Services.KeyGeneration;

namespace LangApp.Core.Factories.Posts;

public class PostCommentFactory : IPostCommentFactory
{
    private readonly IKeyGenerator _keyGenerator;

    public PostCommentFactory(IKeyGenerator keyGenerator)
    {
        _keyGenerator = keyGenerator;
    }

    public PostComment Create(Guid authorId, Guid postId, string content)
    {
        var id = _keyGenerator.NewKey();

        var comment = PostComment.Create(id, authorId, postId, content);
        return comment;
    }
}
