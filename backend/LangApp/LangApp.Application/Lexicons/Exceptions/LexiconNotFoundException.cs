using LangApp.Application.Common.Exceptions;
using LangApp.Core.Exceptions;

namespace LangApp.Application.Lexicons.Exceptions;

public class LexiconNotFoundException : NotFoundException
{
    public Guid Id { get; }

    public LexiconNotFoundException(Guid id) : base($"Lexicon with ID {id} was not found.")
    {
        Id = id;
    }
}