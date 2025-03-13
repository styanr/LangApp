using System.Security.Claims;
using LangApp.Api.Common.Exceptions;
using LangApp.Application.Common.Exceptions;
using LangApp.Core.Enums;

namespace LangApp.Api.Endpoints;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var idString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                       throw new AuthenticationException("User ID claim is missing.");

        return Guid.Parse(idString);
    }

    public static UserRole GetUserRole(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var roleString = principal.FindFirst(ClaimTypes.Role)?.Value ??
                         throw new AuthenticationException("User Role claim is missing");
        if (!Enum.TryParse<UserRole>(roleString, out var role))
        {
            throw new AuthenticationException("User Role claim was not able to be parsed");
        }

        return role;
    }
}