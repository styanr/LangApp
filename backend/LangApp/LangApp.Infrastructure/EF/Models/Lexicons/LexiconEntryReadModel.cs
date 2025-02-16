namespace LangApp.Infrastructure.EF.Models.Lexicons;

public class LexiconEntryReadModel
{
    public Guid Id { get; set; }
    public Guid LexiconId { get; set; }
    public string Term { get; set; }

    public LexiconReadModel Lexicon { get; set; }
    public required IEnumerable<LexiconEntryDefinitionReadModel> Definitions { get; set; }
}