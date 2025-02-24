using LangApp.Application.Common.Commands.Abstractions;
using LangApp.Core.Factories.Lexicons;
using LangApp.Core.Repositories;
using LangApp.Core.ValueObjects;

namespace LangApp.Application.Lexicons.Commands;

public record CreateLexicon(
    string Title,
    string Language,
    Guid OwnerId
) : ICommand<Guid>;

public class CreateLexiconHandler : ICommandHandler<CreateLexicon, Guid>
{
    private readonly ILexiconRepository _repository;
    private readonly ILexiconFactory _factory;

    public CreateLexiconHandler(ILexiconRepository repository, ILexiconFactory factory)
    {
        _repository = repository;
        _factory = factory;
    }

    public async Task<Guid> HandleAsync(CreateLexicon command, CancellationToken cancellationToken)
    {
        var (title, languageValue, userId) = command;

        var language = new Language(languageValue);
        var dictionaryTitle = new LexiconTitle(title);

        var lexicon = _factory.Create(userId, language, dictionaryTitle);
        await _repository.AddAsync(lexicon);

        return lexicon.Id;
    }
}