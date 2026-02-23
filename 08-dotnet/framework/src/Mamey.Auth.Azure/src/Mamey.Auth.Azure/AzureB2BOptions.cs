namespace Mamey.Auth.Azure;

public class AzureB2BOptions : AzureOptions
{
    public static readonly string APPSETTINGS_SECTION = "azure:b2b";
    
    /// <summary>
    /// Enable Azure AD B2B authentication (default: false)
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    public string? SignedOutCallbackPath { get; set; } = "/signout-callback-oidc";
    public string? SignInPolicyId { get; set; }
    public string? SignOutPolicyId { get; set; }
}