using System.Text.Json.Serialization;

namespace Mamey.Auth.Azure;

public class AzureB2COptions : AzureOptions
{
    public static readonly string APPSETTINGS_SECTION = "azure:b2c";
    
    /// <summary>
    /// Enable Azure AD B2C authentication (default: false)
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    public string? RedirectUri { get; set; }
    public string SignedOutCallbackPath { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = string.Empty;
    public string ResetPasswordPolicyId { get; set; } = string.Empty;
    public string EditProfilePolicyId { get; set; } = string.Empty;
}

public class AzureOptions
{
    public static readonly string APPSETTINGS_SECTION = "azure";

    public AzureOptions()
    {
    }

    /// <summary>
    /// Enable Azure AD authentication (default: false)
    /// </summary>
    public bool Enabled { get; set; } = false;
    
    public string? Type { get; set; }
    public string? Auth { get; set; }
    public string? Instance { get; set; }
    public string? TenantId { get; set; }
    public string? Domain { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Authority { get; set; }
    public string? Scopes { get; set; }
    public string? CallbackPath { get; set; }
    
    [JsonPropertyName("b2b")]
    public AzureB2BOptions? AzureB2BOptions { get; set; }
    
    [JsonPropertyName("b2c")]
    public AzureB2COptions? AzureB2COptions { get; set; }
    
    public DownstreamApi? DownstreamApi { get; set; }
    
    public GraphOptions? GraphOptions { get; set; }
}


public class DownstreamApi
{
    public string? BaseUrl { get; set; }
    public string? Scopes { get; set; }
}
