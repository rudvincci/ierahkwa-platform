using System.Security.Claims;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

/// <summary>
/// Provides access to the current user context.
/// </summary>
public interface IContext
{
    /// <summary>
    /// Gets the current user's ID (sub claim from OIDC).
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Gets the current user's display name.
    /// </summary>
    string? DisplayName { get; }

    /// <summary>
    /// Gets the current user's email.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's tenant ID.
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Gets the current request IP address.
    /// </summary>
    string? IpAddress { get; }

    /// <summary>
    /// Gets the current request user agent.
    /// </summary>
    string? UserAgent { get; }

    /// <summary>
    /// Gets the base URL of the current request (scheme + host + path base).
    /// </summary>
    string? BaseUrl { get; }

    string? DeviceType { get; }
    string? DeviceId { get; }
    string? MacAddress { get; }
    string? Platform { get; }
    string? Browser { get; }
    string? OsVersion { get; }
    string? AppVersion { get; }
    string? ScreenResolution { get; }
    string? Language { get; }
    string? Timezone { get; }
    string? Referrer { get; }
    string? NetworkType { get; }
    double? Latitude { get; }
    double? Longitude { get; }

    /// <summary>
    /// Checks if the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the user's claims principal.
    /// </summary>
    ClaimsPrincipal? User { get; }
}
