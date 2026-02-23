namespace Mamey.Auth.Azure.B2C;

public class AzureB2COptions
{
    /// <summary>
    /// Enable Azure AD B2C authentication (default: false)
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    public string Instance { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string SignedOutCallbackPath { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = string.Empty;
    public string ResetPasswordPolicyId { get; set; } = string.Empty;
    public string EditProfilePolicyId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}
