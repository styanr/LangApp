using LangApp.Core.Entities.Dictionaries;

namespace LangApp.Core.Repositories;

public interface ILexiconRepository
{
    Task<Lexicon> GetAsync(Guid id);
    Task AddAsync(Lexicon group);
    Task UpdateAsync(Lexicon group);
    Task DeleteAsync(Lexicon group);
}