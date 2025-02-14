using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Factories.Lexicons;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record AddEntry(
    Guid LexiconId,
    Guid EntryId,
    string Term,
    List<string> Definitions
) : ICommand;

public class AddEntryHandler : ICommandHandler<AddEntry>
{
    private readonly ILexiconRepository _repository;
    private readonly ILexiconEntryFactory _entryFactory;

    public AddEntryHandler(ILexiconRepository repository, ILexiconEntryFactory entryFactory)
    {
        _repository = repository;
        _entryFactory = entryFactory;
    }

    public async Task HandleAsync(AddEntry command, CancellationToken cancellationToken)
    {
        var (lexiconId, entryId, termValue, definitionValues) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        var definitions = definitionValues.Select(d => new Definition(d));

        var term = new Term(termValue);

        var entry = _entryFactory.Create(entryId, term, definitions);
        
        lexicon.AddEntry(entry);
        await _repository.UpdateAsync(lexicon);
    }
}