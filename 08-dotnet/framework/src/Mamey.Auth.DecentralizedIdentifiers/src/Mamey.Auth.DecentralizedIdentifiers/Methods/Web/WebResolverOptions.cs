using System.Collections.Generic;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Web;

/// <summary>
/// Configuration options for DID Web resolver
/// </summary>
public class WebResolverOptions
{
    /// <summary>
    /// Enable caching of resolved DID documents
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Cache TTL in minutes
    /// </summary>
    public int CacheTtlMinutes { get; set; } = 60;
    
    /// <summary>
    /// Require domain verification
    /// </summary>
    public bool RequireDomainVerification { get; set; } = false;
    
    /// <summary>
    /// List of allowed domains (if empty, all domains are allowed)
    /// </summary>
    public List<string> AllowedDomains { get; set; } = new List<string>();
    
    /// <summary>
    /// List of blocked domains
    /// </summary>
    public List<string> BlockedDomains { get; set; } = new List<string>();
    
    /// <summary>
    /// HTTP timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Maximum number of redirects to follow
    /// </summary>
    public int MaxRedirects { get; set; } = 5;
    
    /// <summary>
    /// Require HTTPS for did:web resolution
    /// </summary>
    public bool RequireHttps { get; set; } = true;
    
    /// <summary>
    /// Validate SSL certificates
    /// </summary>
    public bool ValidateSslCertificates { get; set; } = true;
}





