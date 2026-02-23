namespace Mamey.Identity.Azure.Configuration;

/// <summary>
/// Azure AD configuration options for B2B, B2C, and Azure AD authentication.
/// </summary>
public class AzureOptions
{
    public const string APPSETTINGS_SECTION = "AzureAd";

    /// <summary>
    /// Whether Azure authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// The type of Azure authentication (b2b, b2c, or azuread).
    /// </summary>
    public string Type { get; set; } = "azuread";

    /// <summary>
    /// Azure AD instance URL (e.g., https://login.microsoftonline.com/).
    /// </summary>
    public string Instance { get; set; } = "https://login.microsoftonline.com/";

    /// <summary>
    /// Azure AD tenant ID.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD client ID (application ID).
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD client secret.
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD domain (for B2B scenarios).
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// Callback path for authentication.
    /// </summary>
    public string CallbackPath { get; set; } = "/signin-oidc";

    /// <summary>
    /// Signed out callback path.
    /// </summary>
    public string SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";

    /// <summary>
    /// B2C sign-up/sign-in policy ID.
    /// </summary>
    public string SignUpSignInPolicyId { get; set; } = string.Empty;

    /// <summary>
    /// B2C reset password policy ID.
    /// </summary>
    public string ResetPasswordPolicyId { get; set; } = string.Empty;

    /// <summary>
    /// B2C edit profile policy ID.
    /// </summary>
    public string EditProfilePolicyId { get; set; } = string.Empty;

    /// <summary>
    /// Additional scopes for Microsoft Graph API.
    /// </summary>
    public string[] GraphScopes { get; set; } = new[] { "User.Read", "Group.Read.All" };

    /// <summary>
    /// Whether to enable Microsoft Graph integration.
    /// </summary>
    public bool EnableGraph { get; set; } = true;

    /// <summary>
    /// Whether to enable token caching.
    /// </summary>
    public bool EnableTokenCaching { get; set; } = true;

    /// <summary>
    /// Token cache configuration.
    /// </summary>
    public TokenCacheOptions TokenCache { get; set; } = new();
}

/// <summary>
/// Token cache configuration options.
/// </summary>
public class TokenCacheOptions
{
    /// <summary>
    /// Whether to use Redis for token caching.
    /// </summary>
    public bool UseRedis { get; set; } = false;

    /// <summary>
    /// Redis connection string for token caching.
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Token cache expiration time in minutes.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 60;
}
