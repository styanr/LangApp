using LangApp.Core.Entities.Lexicons;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Models.Lexicons;
using LangApp.Infrastructure.EF.Models.StudyGroups;

namespace LangApp.Infrastructure.EF.Models.Users;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public FullNameReadModel FullName { get; set; }
    public string? PictureUrl { get; set; }
    public AppUserRole Role { get; set; }

    public ICollection<StudyGroupReadModel> StudyGroups { get; set; }
    public ICollection<StudyGroupReadModel> ManagedGroups { get; set; }

    public ICollection<LexiconReadModel> Lexicons { get; set; }
}