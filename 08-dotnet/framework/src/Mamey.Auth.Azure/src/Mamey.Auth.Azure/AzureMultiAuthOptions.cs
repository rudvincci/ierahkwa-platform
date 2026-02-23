namespace Mamey.Auth.Azure;

/// <summary>
/// Configuration options for Azure multi-authentication coordination (B2B, B2C, Azure AD)
/// </summary>
public class AzureMultiAuthOptions
{
    /// <summary>
    /// Enable regular Azure AD authentication (default: false)
    /// </summary>
    public bool EnableAzure { get; set; } = false;
    
    /// <summary>
    /// Enable Azure AD B2B authentication (default: false)
    /// </summary>
    public bool EnableAzureB2B { get; set; } = false;
    
    /// <summary>
    /// Enable Azure AD B2C authentication (default: false)
    /// </summary>
    public bool EnableAzureB2C { get; set; } = false;
    
    /// <summary>
    /// Azure authentication policy: how to handle multiple Azure authentication methods
    /// </summary>
    public AzureAuthenticationPolicy Policy { get; set; } = AzureAuthenticationPolicy.EitherOr;
    
    /// <summary>
    /// Azure AD authentication scheme name (default: "AzureAD")
    /// </summary>
    public string AzureScheme { get; set; } = "AzureAD";
    
    /// <summary>
    /// Azure AD B2B authentication scheme name (default: "AzureB2B")
    /// </summary>
    public string AzureB2BScheme { get; set; } = "AzureB2B";
    
    /// <summary>
    /// Azure AD B2C authentication scheme name (default: "AzureB2C")
    /// </summary>
    public string AzureB2CScheme { get; set; } = "AzureB2C";
    
    /// <summary>
    /// Configuration section name for Azure AD options (default: "azure")
    /// </summary>
    public string AzureSectionName { get; set; } = "azure";
    
    /// <summary>
    /// Configuration section name for Azure B2B options (default: "azure:b2b")
    /// </summary>
    public string AzureB2BSectionName { get; set; } = "azure:b2b";
    
    /// <summary>
    /// Configuration section name for Azure B2C options (default: "azure:b2c")
    /// </summary>
    public string AzureB2CSectionName { get; set; } = "azure:b2c";
}

/// <summary>
/// Azure authentication policy for handling multiple Azure authentication methods
/// </summary>
public enum AzureAuthenticationPolicy
{
    /// <summary>
    /// Only Azure AD authentication is allowed
    /// </summary>
    AzureOnly,
    
    /// <summary>
    /// Only Azure AD B2B authentication is allowed
    /// </summary>
    B2BOnly,
    
    /// <summary>
    /// Only Azure AD B2C authentication is allowed
    /// </summary>
    B2COnly,
    
    /// <summary>
    /// Try Azure AD first, then B2B, then B2C if previous fails (either/or)
    /// </summary>
    EitherOr,
    
    /// <summary>
    /// All enabled Azure methods are required (rare use case)
    /// </summary>
    AllRequired
}


