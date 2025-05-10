using LangApp.Core.Entities.Posts;

namespace LangApp.Core.Factories.Posts;

public interface IPostCommentFactory
{
    PostComment Create(Guid authorId, Guid postId, string content);
}
