using LangApp.Core.Enums;
using LangApp.Infrastructure.EF.Models.StudyGroups;

namespace LangApp.Infrastructure.EF.Models.Users;

public class UserReadModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public FullNameReadModel FullName { get; set; }
    public string Email { get; set; }
    public string? PictureUrl { get; set; }
    public UserRole Role { get; set; }

    public string? NormalizedUserName { get; set; }


    public virtual string? NormalizedEmail { get; set; }
    public virtual bool EmailConfirmed { get; set; }
    public virtual string? PasswordHash { get; set; }
    public virtual string? SecurityStamp { get; set; }
    public virtual string? ConcurrencyStamp { get; set; }
    public virtual string? PhoneNumber { get; set; }
    public virtual bool PhoneNumberConfirmed { get; set; }
    public virtual bool TwoFactorEnabled { get; set; }
    public virtual DateTimeOffset? LockoutEnd { get; set; }
    public virtual bool LockoutEnabled { get; set; }
    public virtual int AccessFailedCount { get; set; }
    public ICollection<StudyGroupReadModel> StudyGroups { get; set; }
    public ICollection<StudyGroupReadModel> ManagedGroups { get; set; }
}
