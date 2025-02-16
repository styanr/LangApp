using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Models.Lexicons;

public class LexiconReadModel
{
    public Guid Id { get; init; }
    public string Language { get; init; }
    public string Title { get; init; }
    public Guid OwnerId { get; set; }

    public UserReadModel Owner { get; set; }
    public List<LexiconEntryReadModel> Entries { get; set; } = new();
}