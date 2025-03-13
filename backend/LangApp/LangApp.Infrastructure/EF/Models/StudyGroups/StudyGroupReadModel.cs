using LangApp.Core.Common;
using LangApp.Infrastructure.EF.Models.Posts;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Models.StudyGroups;

public class StudyGroupReadModel : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; }
    public string Language { get; set; }
    public UserReadModel Owner { get; set; }
    public ICollection<UserReadModel> Members { get; set; }
    public ICollection<PostReadModel> Posts { get; set; }
}