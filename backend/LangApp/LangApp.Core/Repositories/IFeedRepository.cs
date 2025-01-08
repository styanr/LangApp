using LangApp.Core.Entities.Feeds;

namespace LangApp.Core.Repositories;

public interface IFeedRepository
{
    Task<Feed> GetAsync(Guid id);
    Task AddAsync(Feed feed);
    Task UpdateAsync(Feed feed);
    Task DeleteAsync(Feed feed);
}