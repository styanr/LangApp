using LangApp.Application.Common.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record AddDefinition(
    Guid LexiconId,
    string Expression,
    string Definition
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
        var (lexiconId, expressionValue, definitionValue) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        var expression = new Expression(expressionValue);
        var definition = new Definition(definitionValue);

        lexicon.AddDefinition(expression, definition);
        await _repository.UpdateAsync(lexicon);
    }
}