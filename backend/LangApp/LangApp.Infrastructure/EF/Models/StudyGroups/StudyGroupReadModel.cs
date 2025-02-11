using LangApp.Application.Posts.Dto;
using LangApp.Infrastructure.EF.Models.Users;

namespace LangApp.Application.StudyGroups.Dto;

public class StudyGroupReadModel
{
    public string Name { get; set; }
    public string Language { get; set; }
    public UserReadModel Owner { get; set; }
    public ICollection<UserReadModel> Members { get; set; }
    public ICollection<PostReadModel> Posts { get; set; }
}