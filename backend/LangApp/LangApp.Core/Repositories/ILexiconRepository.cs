using LangApp.Core.Entities.Lexicons;

namespace LangApp.Core.Repositories;

public interface ILexiconRepository
{
    Task<Lexicon?> GetAsync(Guid id);
    Task AddAsync(Lexicon lexicon);
    Task UpdateAsync(Lexicon lexicon);
    Task DeleteAsync(Lexicon lexicon);
}