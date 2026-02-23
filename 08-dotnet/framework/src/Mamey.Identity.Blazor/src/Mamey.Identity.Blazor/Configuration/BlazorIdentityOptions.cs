namespace Mamey.Identity.Blazor.Configuration;

/// <summary>
/// Configuration options for Blazor WebAssembly authentication.
/// </summary>
public class BlazorIdentityOptions
{
    public const string SectionName = "Identity:Blazor";

    /// <summary>
    /// Gets or sets whether Blazor authentication is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the API base URL for authentication endpoints.
    /// </summary>
    public string ApiBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the login endpoint path.
    /// </summary>
    public string LoginEndpoint { get; set; } = "/api/auth/login";

    /// <summary>
    /// Gets or sets the logout endpoint path.
    /// </summary>
    public string LogoutEndpoint { get; set; } = "/api/auth/logout";

    /// <summary>
    /// Gets or sets the refresh token endpoint path.
    /// </summary>
    public string RefreshTokenEndpoint { get; set; } = "/api/auth/refresh";

    /// <summary>
    /// Gets or sets the user info endpoint path.
    /// </summary>
    public string UserInfoEndpoint { get; set; } = "/api/auth/user";

    /// <summary>
    /// Gets or sets the token storage key in localStorage.
    /// </summary>
    public string TokenStorageKey { get; set; } = "mamey_identity_token";

    /// <summary>
    /// Gets or sets the refresh token storage key in localStorage.
    /// </summary>
    public string RefreshTokenStorageKey { get; set; } = "mamey_identity_refresh_token";

    /// <summary>
    /// Gets or sets the user info storage key in localStorage.
    /// </summary>
    public string UserInfoStorageKey { get; set; } = "mamey_identity_user";

    /// <summary>
    /// Gets or sets whether to automatically refresh tokens.
    /// </summary>
    public bool AutoRefreshTokens { get; set; } = true;

    /// <summary>
    /// Gets or sets the token refresh interval in minutes.
    /// </summary>
    public int TokenRefreshIntervalMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the token expiration threshold in minutes.
    /// </summary>
    public int TokenExpirationThresholdMinutes { get; set; } = 10;

    /// <summary>
    /// Gets or sets whether to persist authentication state across browser sessions.
    /// </summary>
    public bool PersistAuthenticationState { get; set; } = true;

    /// <summary>
    /// Gets or sets the login redirect URL.
    /// </summary>
    public string LoginRedirectUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the logout redirect URL.
    /// </summary>
    public string LogoutRedirectUrl { get; set; } = "/";

    /// <summary>
    /// Gets or sets the access denied redirect URL.
    /// </summary>
    public string AccessDeniedUrl { get; set; } = "/access-denied";

    /// <summary>
    /// Gets or sets whether to show loading indicators during authentication.
    /// </summary>
    public bool ShowLoadingIndicators { get; set; } = true;

    /// <summary>
    /// Gets or sets the loading indicator text.
    /// </summary>
    public string LoadingText { get; set; } = "Authenticating...";

    /// <summary>
    /// Gets or sets whether to enable debug logging.
    /// </summary>
    public bool EnableDebugLogging { get; set; } = false;
}


































