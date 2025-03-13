using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Common.Exceptions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record AddDefinition(
    Guid LexiconId,
    Guid EntryId,
    string Definition,
    Guid UserId
) : ICommand;

public class AddDefinitionHandler : ICommandHandler<AddDefinition>
{
    private readonly ILexiconRepository _repository;

    public AddDefinitionHandler(ILexiconRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddDefinition command, CancellationToken cancellationToken)
    {
        var (lexiconId, entryId, definitionValue, userId) = command;

        var lexicon = await _repository.GetAsync(lexiconId)
                      ?? throw new LexiconNotFoundException(lexiconId);

        if (!lexicon.CanBeModifiedBy(userId))
        {
            throw new UnauthorizedException(userId, lexicon);
        }

        var definition = new Definition(definitionValue);

        lexicon.AddDefinition(entryId, definition);
        await _repository.UpdateAsync(lexicon);
    }
}