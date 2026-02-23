using Microsoft.AspNetCore.Authorization;

namespace Mamey.Auth.Decentralized;

/// <summary>
/// Attribute for requiring decentralized authentication
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DecentralizedAuthAttribute : Attribute, IAuthorizeData
{
    /// <summary>
    /// Gets or sets the policy name that determines access to the resource
    /// </summary>
    public string? Policy { get; set; }
    
    /// <summary>
    /// Gets or sets a comma delimited list of roles that are allowed to access the resource
    /// </summary>
    public string? Roles { get; set; }
    
    /// <summary>
    /// Gets or sets a comma delimited list of schemes from which user information is constructed
    /// </summary>
    public string? AuthenticationSchemes { get; set; }
    
    /// <summary>
    /// Gets or sets the required DID method
    /// </summary>
    public string? RequiredMethod { get; set; }
    
    /// <summary>
    /// Gets or sets the required verification method type
    /// </summary>
    public string? RequiredVerificationMethodType { get; set; }
    
    /// <summary>
    /// Gets or sets whether to require a valid DID signature
    /// </summary>
    public bool RequireSignature { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to require a valid DID Document
    /// </summary>
    public bool RequireValidDidDocument { get; set; } = true;
    
    /// <summary>
    /// Gets or sets whether to require a specific issuer
    /// </summary>
    public string? RequiredIssuer { get; set; }
    
    /// <summary>
    /// Gets or sets whether to require a specific audience
    /// </summary>
    public string? RequiredAudience { get; set; }
    
    /// <summary>
    /// Gets or sets the minimum key strength in bits
    /// </summary>
    public int? MinimumKeyStrength { get; set; }
    
    /// <summary>
    /// Gets or sets whether to allow expired DIDs
    /// </summary>
    public bool AllowExpired { get; set; } = false;
    
    /// <summary>
    /// Gets or sets whether to allow deactivated DIDs
    /// </summary>
    public bool AllowDeactivated { get; set; } = false;
}
