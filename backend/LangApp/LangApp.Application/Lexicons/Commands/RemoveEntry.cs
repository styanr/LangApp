using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record RemoveEntry(
    Guid LexiconId,
    Guid EntryId,
    Guid UserId
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
        var (lexiconId, entryId, userId) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        if (!lexicon.CanBeModifiedBy(userId))
        {
            throw new UnauthorizedException(userId, lexicon);
        }

        lexicon.RemoveEntry(entryId);
        await _repository.UpdateAsync(lexicon);
    }
}