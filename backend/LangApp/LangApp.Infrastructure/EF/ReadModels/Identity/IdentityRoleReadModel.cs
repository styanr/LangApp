namespace LangApp.Infrastructure.EF.Models.Identity;

public class IdentityRoleReadModel
{
    public Guid Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual string? NormalizedName { get; set; }
    public virtual string? ConcurrencyStamp { get; set; }
}