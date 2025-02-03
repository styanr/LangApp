using LangApp.Core.Entities.Posts;

namespace LangApp.Core.Repositories;

public interface IPostRepository
{
    Task<Post?> GetAsync(Guid id);
    Task AddAsync(Post group);
    Task UpdateAsync(Post group);
    Task DeleteAsync(Post group);
}