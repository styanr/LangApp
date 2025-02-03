using LangApp.Application.Common.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record AddEntry(
    Guid LexiconId,
    string Expression,
    List<string> Definitions
) : ICommand;

public class AddEntryHandler : ICommandHandler<AddEntry>
{
    private readonly ILexiconRepository _repository;

    public AddEntryHandler(ILexiconRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(AddEntry command, CancellationToken cancellationToken)
    {
        var (lexiconId, expressionValue, definitionValues) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        var expression = new Expression(expressionValue);
        var definitions = new Definitions(definitionValues.Select(d => new Definition(d)));

        lexicon.AddEntry(expression, definitions);
        await _repository.UpdateAsync(lexicon);
    }
}