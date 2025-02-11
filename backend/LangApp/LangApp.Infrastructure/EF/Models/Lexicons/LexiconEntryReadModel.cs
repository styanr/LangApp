namespace LangApp.Infrastructure.EF.Models.Lexicons;

public class LexiconEntryReadModel
{
    public Guid Id { get; set; }
    public string Term { get; set; }
    public ICollection<string> Definitions { get; set; }

    public LexiconReadModel Lexicon { get; set; }
}