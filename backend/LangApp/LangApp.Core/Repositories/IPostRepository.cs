using LangApp.Core.Entities.Posts;

namespace LangApp.Core.Repositories;

public interface IPostRepository
{
    Task<Post?> GetAsync(Guid id);
    Task AddAsync(Post post);
    Task UpdateAsync(Post post);
    Task DeleteAsync(Post post);
}