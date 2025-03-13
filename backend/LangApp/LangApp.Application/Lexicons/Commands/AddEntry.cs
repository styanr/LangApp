using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Factories.Lexicons;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record AddEntry(
    Guid LexiconId,
    string Term,
    List<string> Definitions,
    Guid UserId
) : ICommand<Guid>;

public class AddEntryHandler : ICommandHandler<AddEntry, Guid>
{
    private readonly ILexiconRepository _repository;
    private readonly ILexiconEntryFactory _entryFactory;

    public AddEntryHandler(ILexiconRepository repository, ILexiconEntryFactory entryFactory)
    {
        _repository = repository;
        _entryFactory = entryFactory;
    }

    public async Task<Guid> HandleAsync(AddEntry command, CancellationToken cancellationToken)
    {
        var (lexiconId, termValue, definitionValues, userId) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        if (!lexicon.CanBeModifiedBy(userId))
        {
            throw new UnauthorizedException(userId, lexicon);
        }
        
        var definitions = definitionValues.Select(d => new Definition(d));

        var term = new Term(termValue);

        var entry = _entryFactory.Create(lexiconId, term, definitions);

        lexicon.AddEntry(entry);
        await _repository.UpdateAsync(lexicon);

        return entry.Id;
    }
}