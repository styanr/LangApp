using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Application.Lexicons.Dto;

public class LexiconReadModel
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }


    public string Language { get; init; }
    public string Title { get; init; }
    public IDictionary<string, IEnumerable<string>> Entries { get; init; }

    public UserReadModel Owner { get; set; }
}