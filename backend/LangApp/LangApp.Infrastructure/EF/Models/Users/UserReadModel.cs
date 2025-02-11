using LangApp.Core.Enums;

namespace LangApp.Infrastructure.EF.Models.Users;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public FullNameReadModel FullName { get; set; }
    public string? PictureUrl { get; set; }
    public AppUserRole Role { get; set; }
}