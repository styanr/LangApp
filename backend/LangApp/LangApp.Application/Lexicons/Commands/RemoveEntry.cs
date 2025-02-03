using LangApp.Application.Common.Abstractions;
using LangApp.Application.Lexicons.Exceptions;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record RemoveEntry(
    Guid LexiconId,
    string Expression
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
        var (lexiconId, expressionValue) = command;

        var lexicon = await _repository.GetAsync(lexiconId) ?? throw new LexiconNotFoundException(lexiconId);

        var expression = new Expression(expressionValue);

        lexicon.RemoveEntry(expression);
        await _repository.UpdateAsync(lexicon);
    }
}