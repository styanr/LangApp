using LangApp.Core.Common;
using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Models.StudyGroups;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Infrastructure.EF.Models.Posts;

public class PostReadModel : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid GroupId { get; set; }

    public bool Archived { get; private set; }
    public PostType Type { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime EditedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Media { get; set; }

    public UserReadModel Author { get; set; }
    public StudyGroupReadModel Group { get; set; }

    public bool IsEdited => CreatedAt != EditedAt;
}