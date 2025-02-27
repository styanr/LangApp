namespace LangApp.Infrastructure.EF.Options;

public class JwtOptions
{
    public const string Section = "JwtOptions";

    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int Expiry { get; init; }
    public int RefreshExpiry { get; init; }
}