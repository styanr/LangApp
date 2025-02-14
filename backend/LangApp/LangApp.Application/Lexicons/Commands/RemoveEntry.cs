using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record RemoveEntry(
    Guid LexiconId,
    Guid EntryId
) : ICommand;

public class RemoveEntryHandler : ICommandHandler<RemoveEntry>
{
    private readonly ILexiconRepository _repository;

    public RemoveEntryHandler(ILexiconRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveEntry command, CancellationToken cancellationToken)
    {
        var (lexiconId, entryId) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        lexicon.RemoveEntry(entryId);
        await _repository.UpdateAsync(lexicon);
    }
}