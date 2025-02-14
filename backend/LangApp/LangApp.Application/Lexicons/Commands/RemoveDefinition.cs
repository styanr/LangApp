using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record RemoveDefinition(
    Guid LexiconId,
    Guid EntryId,
    string Definition
) : ICommand;

public class RemoveDefinitionHandler : ICommandHandler<RemoveDefinition>
{
    private readonly ILexiconRepository _repository;

    public RemoveDefinitionHandler(ILexiconRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(RemoveDefinition command, CancellationToken cancellationToken)
    {
        var (lexiconId, entryId, definitionValue) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        var definition = new Definition(definitionValue);
        lexicon.RemoveDefinition(entryId, definition);

        await _repository.UpdateAsync(lexicon);
    }
}