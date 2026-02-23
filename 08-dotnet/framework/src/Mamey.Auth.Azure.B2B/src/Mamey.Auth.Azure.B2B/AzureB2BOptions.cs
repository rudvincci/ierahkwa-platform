namespace Mamey.Auth.Azure.B2B;

public class AzureB2BOptions
{
    /// <summary>
    /// Enable Azure AD B2B authentication (default: false)
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Instance { get; set; } = "https://login.microsoftonline.com/";
    public string? Domain { get; set; }
    public string? CallbackPath { get; set; } = "/signin-oidc";
    public string? SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    public string? SignInPolicyId { get; set; }
    public string? SignOutPolicyId { get; set; }
    public string[] Scopes { get; set; }
    public string? Authority { get; set; }
}
