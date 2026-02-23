using System.Security.Claims;

namespace Mamey.Government.Modules.Identity.Blazor.Services;

/// <summary>
/// Service for authentication operations with Authentik OIDC.
/// </summary>
public interface IIdentityAuthService
{
    /// <summary>
    /// Gets the current authenticated user.
    /// </summary>
    Task<ClaimsPrincipal?> GetCurrentUserAsync();

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    Task<bool> IsAuthenticatedAsync();

    /// <summary>
    /// Gets the user's display name.
    /// </summary>
    Task<string?> GetDisplayNameAsync();

    /// <summary>
    /// Gets the user's email.
    /// </summary>
    Task<string?> GetEmailAsync();

    /// <summary>
    /// Gets the user's tenant ID.
    /// </summary>
    Task<Guid?> GetTenantIdAsync();

    /// <summary>
    /// Signs out the current user.
    /// </summary>
    Task SignOutAsync();
}
