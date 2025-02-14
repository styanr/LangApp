using LangApp.Core.Enums;
using LangApp.Core.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace LangApp.Infrastructure.EF.Models.Identity;

public class IdentityApplicationUser : IdentityUser
{
    public Username Username { get; private set; }
    public string Email { get; private set; }
    public UserFullName FullName { get; private set; }
    public string? PictureUrl { get; private set; }
    public AppUserRole Role { get; private set; }
}