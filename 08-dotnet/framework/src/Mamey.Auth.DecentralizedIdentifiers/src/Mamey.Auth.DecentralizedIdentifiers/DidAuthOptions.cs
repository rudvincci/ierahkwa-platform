using System.Collections.Generic;

namespace Mamey.Auth.DecentralizedIdentifiers;

/// <summary>
/// Configuration options for DID authentication
/// </summary>
public class DidAuthOptions
{
    public const string SectionName = "didAuth";
    
    /// <summary>
    /// List of supported DID methods (key, web, ethr, ion, peer)
    /// </summary>
    public List<string> EnabledMethods { get; set; } = new() { "key", "web", "ethr" };
    
    /// <summary>
    /// Require cryptographic proof for authentication
    /// </summary>
    public bool RequireProof { get; set; } = true;
    
    /// <summary>
    /// Enable VP validation
    /// </summary>
    public bool ValidatePresentation { get; set; } = true;
    
    /// <summary>
    /// DID document caching configuration
    /// </summary>
    public DidCacheOptions CacheOptions { get; set; } = new();
    
    /// <summary>
    /// Resolver endpoints per method
    /// </summary>
    public Dictionary<string, string> ResolverEndpoints { get; set; } = new();
    
    /// <summary>
    /// Trust registry endpoint
    /// </summary>
    public string TrustRegistryUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Enable credential revocation checks
    /// </summary>
    public bool RevocationCheckEnabled { get; set; } = true;
    
    /// <summary>
    /// Cookie-based authentication settings
    /// </summary>
    public DidCookieOptions CookieOptions { get; set; } = new();
    
    /// <summary>
    /// Authentication settings
    /// </summary>
    public AuthenticationOptions Authentication { get; set; } = new();
    
    /// <summary>
    /// Key storage configuration
    /// </summary>
    public KeyStorageOptions KeyStorage { get; set; } = new();
    
    /// <summary>
    /// JWT issuer
    /// </summary>
    public string Issuer { get; set; } = "did-auth";
    
    /// <summary>
    /// JWT audience
    /// </summary>
    public string Audience { get; set; } = "did-auth-audience";
    
    /// <summary>
    /// Anonymous endpoints that don't require authentication
    /// </summary>
    public List<string> AllowAnonymousEndpoints { get; set; } = new();
    
    /// <summary>
    /// Cache options for DID documents and verification results
    /// </summary>
    public class DidCacheOptions
    {
        /// <summary>
        /// Enable caching
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Cache TTL in minutes
        /// </summary>
        public int TtlMinutes { get; set; } = 60;
        
        /// <summary>
        /// Storage type for cache (Memory, Redis)
        /// </summary>
        public string StorageType { get; set; } = "Memory";
        
        /// <summary>
        /// Redis connection string (if using Redis)
        /// </summary>
        public string RedisConnectionString { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Cookie authentication options
    /// </summary>
    public class DidCookieOptions
    {
        /// <summary>
        /// Cookie name
        /// </summary>
        public string Name { get; set; } = "__did-access-token";
        
        /// <summary>
        /// HTTP only cookie
        /// </summary>
        public bool HttpOnly { get; set; } = true;
        
        /// <summary>
        /// Secure cookie (HTTPS only)
        /// </summary>
        public bool Secure { get; set; } = true;
        
        /// <summary>
        /// Same site policy
        /// </summary>
        public string SameSite { get; set; } = "Strict";
        
        /// <summary>
        /// Cookie expiration in minutes
        /// </summary>
        public int ExpirationMinutes { get; set; } = 60;
    }
    
    /// <summary>
    /// Authentication-specific options
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        /// Challenge lifetime in minutes
        /// </summary>
        public int ChallengeLifetimeMinutes { get; set; } = 10;
        
        /// <summary>
        /// Allowed clock skew in seconds
        /// </summary>
        public int AllowedClockSkewSeconds { get; set; } = 300;
        
        /// <summary>
        /// Require domain binding for presentations
        /// </summary>
        public bool RequireDomainBinding { get; set; } = true;
        
        /// <summary>
        /// Require challenge for authentication
        /// </summary>
        public bool RequireChallenge { get; set; } = true;
        
        /// <summary>
        /// Token expiration in minutes
        /// </summary>
        public int TokenExpirationMinutes { get; set; } = 60;
    }
    
    /// <summary>
    /// Key storage options
    /// </summary>
    public class KeyStorageOptions
    {
        /// <summary>
        /// Storage provider (AzureKeyVault, Local, HSM)
        /// </summary>
        public string Provider { get; set; } = "Local";
        
        /// <summary>
        /// Azure Key Vault URL
        /// </summary>
        public string VaultUrl { get; set; } = string.Empty;
        
        /// <summary>
        /// Enable HSM for key generation
        /// </summary>
        public bool EnableHsm { get; set; } = false;
        
        /// <summary>
        /// Local key encryption key
        /// </summary>
        public string EncryptionKey { get; set; } = string.Empty;
        
        /// <summary>
        /// Key rotation grace period in days
        /// </summary>
        public int KeyRotationGracePeriodDays { get; set; } = 30;
    }
}





