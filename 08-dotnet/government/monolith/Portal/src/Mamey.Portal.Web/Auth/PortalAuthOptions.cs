namespace Mamey.Portal.Web.Auth;

public sealed class PortalAuthOptions
{
    /// <summary>
    /// Authentication mode: "Mock" (default) or "Oidc".
    /// </summary>
    public string Mode { get; init; } = "Mock";

    public OidcAuthOptions Oidc { get; init; } = new();

    /// <summary>
    /// Claim type that contains the tenant id (defaults to "tenant").
    /// </summary>
    public string TenantClaimType { get; init; } = "tenant";
}

public sealed class OidcAuthOptions
{
    /// <summary>
    /// OIDC authority/issuer base URL. For authentik this is typically:
    /// http://localhost:9100/application/o/{provider-slug}/
    /// </summary>
    public string Authority { get; init; } = string.Empty;

    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// Claim used for roles (e.g. "roles" or "groups"). Defaults to "roles".
    /// </summary>
    public string RoleClaimType { get; init; } = "roles";

    /// <summary>
    /// Claim used for display name / username. Defaults to "preferred_username".
    /// </summary>
    public string NameClaimType { get; init; } = "preferred_username";

    /// <summary>
    /// Development convenience: allow HTTP metadata in non-production.
    /// </summary>
    public bool RequireHttpsMetadata { get; init; } = false;
}




