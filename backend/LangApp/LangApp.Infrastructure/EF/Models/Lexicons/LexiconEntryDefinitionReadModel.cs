namespace LangApp.Infrastructure.EF.Models.Lexicons;

public class LexiconEntryDefinitionReadModel
{
    public Guid Id { get; set; }
    public Guid LexiconEntryId { get; set; }
    public string Value { get; set; }
    public LexiconEntryReadModel Entry { get; set; }
}