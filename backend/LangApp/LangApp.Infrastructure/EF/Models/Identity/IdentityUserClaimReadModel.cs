namespace LangApp.Infrastructure.EF.Models.Identity;

public class IdentityUserClaimReadModel
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }
}