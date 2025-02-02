using LangApp.Core.Entities.Dictionaries;

namespace LangApp.Core.Repositories;

public interface IDictionaryRepository
{
    Task<LanguageDictionary> GetAsync(Guid id);
    Task AddAsync(LanguageDictionary group);
    Task UpdateAsync(LanguageDictionary group);
    Task DeleteAsync(LanguageDictionary group);
}