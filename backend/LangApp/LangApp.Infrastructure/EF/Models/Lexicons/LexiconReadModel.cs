using LangApp.Core.Common;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Models.Lexicons;

public class LexiconReadModel : IIdentifiable
{
    public Guid Id { get; init; }
    public string Language { get; init; }
    public string Title { get; init; }
    public Guid UserId { get; set; }

    public UserReadModel Owner { get; set; }
    public required IEnumerable<LexiconEntryReadModel> Entries { get; set; }
}