namespace Mamey.Identity.AspNetCore;

public static class ClaimCategory
{
    /// <summary>Low‑level permissions you grant to a user (e.g. "Identity.Read").</summary>
    public const string Permission   = "Permission";

    /// <summary>Application roles (e.g. "SystemAdmin", "User").</summary>
    public const string Role         = "Role";

    /// <summary>Feature‑flag or feature‑access markers (e.g. "BetaFeatureX").</summary>
    public const string Feature      = "Feature";

    /// <summary>OAuth/OIDC scopes (e.g. "openid", "profile", "email").</summary>
    public const string Scope        = "Scope";

    /// <summary>Organizational grouping (e.g. "Department", "BusinessUnit").</summary>
    public const string Department   = "Department";

    /// <summary>System‑level flags not tied to any business domain (e.g. "SystemMaintenance").</summary>
    public const string System       = "System";

    /// <summary>User preferences (e.g. "Theme", "Locale").</summary>
    public const string Preference   = "Preference";

    /// <summary>Ad‑hoc group memberships (e.g. "ProjectXTeam", "AuditCommittee").</summary>
    public const string Group        = "Group";

    /// <summary>High‑level policies (e.g. "DataExportAllowed").</summary>
    public const string Policy       = "Policy";

    /// <summary>CRUD rights on specific resources (e.g. "Customer.Create", "Invoice.Delete").</summary>
    public const string Resource     = "Resource";

    /// <summary>Environment‑specific indicators (e.g. "Environment=Production").</summary>
    public const string Environment  = "Environment";

    /// <summary>Authentication methods used (e.g. "AuthMethod=2FA", "AuthMethod=Password").</summary>
    public const string Authentication = "Authentication";

    /// <summary>Identity provider info (e.g. "IdP=AzureAD", "IdP=Google").</summary>
    public const string IdentityProvider = "IdentityProvider";

    /// <summary>Localization settings (e.g. "Locale=en-US").</summary>
    public const string Locale       = "Locale";

    /// <summary>Time‑based claims (e.g. "IssuedAt", "ExpiresAt").</summary>
    public const string Time         = "Time";
    
    /// <summary> Name/email claims </summary>
    public const string Profile       = "Profile";
}
