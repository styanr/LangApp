namespace LangApp.Infrastructure.EF.Identity;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = null!;
    public Guid UserId { get; set; }
    public IdentityApplicationUser User { get; set; } = null!;
    public DateTime ExpiresAtUtc { get; set; }
    public bool IsRevoked { get; set; } = false;
}
