using Microsoft.AspNetCore.Identity;

namespace LangApp.Infrastructure.EF.Models.Identity;

public class IdentityUserLoginReadModel
{
    public class IdentityUserLogin<TKey> where TKey : IEquatable<TKey>
    {
        public string LoginProvider { get; set; } = default!;
        public string ProviderKey { get; set; } = default!;
        public string? ProviderDisplayName { get; set; }
        public Guid UserId { get; set; } = default!;
    }
}