using LangApp.Core.Entities.Lexicons;
using LangApp.Core.ValueObjects;

namespace LangApp.Core.Factories.Lexicons;

public interface ILexiconEntryFactory
{
    LexiconEntry Create(Term term, IEnumerable<Definition> definitions);
}