using System.Security.Claims;
using Mamey.Auth.Identity.Constants;

namespace Mamey.Auth.Identity;

/// <summary>
/// Helpers for reading typed values from a <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var val = user.FindFirst(IdentityClaims.Subject)?.Value
                  ?? throw new InvalidOperationException("Claim 'sub' not found.");
        return Guid.Parse(val);
    }

    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var val = user.FindFirst(IdentityClaims.TenantId)?.Value
                  ?? throw new InvalidOperationException("Claim 'tenantId' not found.");
        return Guid.Parse(val);
    }

    public static string? GetFullName(this ClaimsPrincipal user)
        => user.FindFirst(IdentityClaims.FullName)?.Value;
}