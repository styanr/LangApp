using LangApp.Core.Entities.Posts;

namespace LangApp.Core.Repositories;

public interface IPostRepository
{
    Task<Post?> GetAsync(Guid id);
    Task<Post?> GetAsync(Guid id, bool showArchived);
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}