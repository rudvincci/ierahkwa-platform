namespace Mamey.Auth.Multi;

/// <summary>
/// Configuration options for multi-authentication coordination (JWT, DID, Azure, Identity, Distributed, Certificate)
/// </summary>
public class MultiAuthOptions
{
    /// <summary>
    /// Enable JWT authentication (default: false)
    /// </summary>
    public bool EnableJwt { get; set; } = false;
    
    /// <summary>
    /// Enable DID (Decentralized Identifier) authentication (default: false)
    /// </summary>
    public bool EnableDid { get; set; } = false;
    
    /// <summary>
    /// Enable Azure authentication (default: false)
    /// </summary>
    public bool EnableAzure { get; set; } = false;
    
    /// <summary>
    /// Enable Identity authentication (default: false)
    /// </summary>
    public bool EnableIdentity { get; set; } = false;
    
    /// <summary>
    /// Enable Distributed authentication (default: false)
    /// </summary>
    public bool EnableDistributed { get; set; } = false;
    
    /// <summary>
    /// Enable Certificate authentication (default: false)
    /// </summary>
    public bool EnableCertificate { get; set; } = false;
    
    /// <summary>
    /// Authentication policy: how to handle multiple authentication methods
    /// </summary>
    public AuthenticationPolicy Policy { get; set; } = AuthenticationPolicy.EitherOr;
    
    /// <summary>
    /// JWT authentication scheme name (default: "Bearer")
    /// </summary>
    public string JwtScheme { get; set; } = "Bearer";
    
    /// <summary>
    /// DID authentication scheme name (default: "DidBearer")
    /// </summary>
    public string DidScheme { get; set; } = "DidBearer";
    
    /// <summary>
    /// Azure authentication scheme name (default: "AzureAD")
    /// </summary>
    public string AzureScheme { get; set; } = "AzureAD";
    
    /// <summary>
    /// Identity authentication scheme name (default: "Identity")
    /// </summary>
    public string IdentityScheme { get; set; } = "Identity";
    
    /// <summary>
    /// Distributed authentication scheme name (default: "Distributed")
    /// </summary>
    public string DistributedScheme { get; set; } = "Distributed";
    
    /// <summary>
    /// Certificate authentication scheme name (default: "Certificate")
    /// </summary>
    public string CertificateScheme { get; set; } = "Certificate";
    
    /// <summary>
    /// Configuration section name for JWT options (default: "jwt")
    /// </summary>
    public string JwtSectionName { get; set; } = "jwt";
    
    /// <summary>
    /// Configuration section name for DID options (default: "didAuth")
    /// </summary>
    public string DidSectionName { get; set; } = "didAuth";
    
    /// <summary>
    /// Configuration section name for Azure options (default: "azure")
    /// </summary>
    public string AzureSectionName { get; set; } = "azure";
    
    /// <summary>
    /// Configuration section name for Identity options (default: "auth")
    /// </summary>
    public string IdentitySectionName { get; set; } = "auth";
    
    /// <summary>
    /// Configuration section name for Distributed options (default: "distributedAuth")
    /// </summary>
    public string DistributedSectionName { get; set; } = "distributedAuth";
    
    /// <summary>
    /// Configuration section name for Certificate options (default: "certificateAuth")
    /// </summary>
    public string CertificateSectionName { get; set; } = "certificateAuth";
}

/// <summary>
/// Authentication policy for handling multiple authentication methods
/// </summary>
public enum AuthenticationPolicy
{
    /// <summary>
    /// Only JWT authentication is allowed
    /// </summary>
    JwtOnly,
    
    /// <summary>
    /// Only DID authentication is allowed
    /// </summary>
    DidOnly,
    
    /// <summary>
    /// Only Azure authentication is allowed
    /// </summary>
    AzureOnly,
    
    /// <summary>
    /// Only Identity authentication is allowed
    /// </summary>
    IdentityOnly,
    
    /// <summary>
    /// Only Distributed authentication is allowed
    /// </summary>
    DistributedOnly,
    
    /// <summary>
    /// Only Certificate authentication is allowed
    /// </summary>
    CertificateOnly,
    
    /// <summary>
    /// Try authentication methods in order (JWT -> DID -> Azure -> Identity -> Distributed -> Certificate) until one succeeds (either/or)
    /// </summary>
    EitherOr,
    
    /// <summary>
    /// Try authentication methods in priority order (JWT first, then DID, then Azure, etc.)
    /// </summary>
    PriorityOrder,
    
    /// <summary>
    /// All enabled authentication methods are required (rare use case)
    /// </summary>
    AllRequired
}


