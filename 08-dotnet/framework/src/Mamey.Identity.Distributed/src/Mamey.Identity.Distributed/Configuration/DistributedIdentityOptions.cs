using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Mamey.Identity.Distributed.Configuration;

/// <summary>
/// Configuration options for distributed identity services.
/// </summary>
public class DistributedIdentityOptions
{
    public const string SectionName = "Identity:Distributed";

    /// <summary>
    /// Gets or sets whether distributed identity is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets whether JWT authentication is enabled for distributed scenarios.
    /// </summary>
    public bool EnableJwtAuthentication { get; set; } = true;

    /// <summary>
    /// Gets or sets whether distributed session management is enabled.
    /// </summary>
    public bool EnableDistributedSessions { get; set; } = true;

    /// <summary>
    /// Gets or sets whether microservice authentication is enabled.
    /// </summary>
    public bool EnableMicroserviceAuth { get; set; } = true;

    /// <summary>
    /// Gets or sets the JWT issuer.
    /// </summary>
    public string JwtIssuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT audience.
    /// </summary>
    public string JwtAudience { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT signing key.
    /// </summary>
    public string JwtSigningKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the JWT expiration time in minutes.
    /// </summary>
    public int JwtExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Gets or sets the session timeout in minutes.
    /// </summary>
    public int SessionTimeoutMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the token refresh threshold in minutes.
    /// </summary>
    public int TokenRefreshThresholdMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets whether to validate token expiration.
    /// </summary>
    public bool ValidateTokenExpiration { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate token issuer.
    /// </summary>
    public bool ValidateTokenIssuer { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate token audience.
    /// </summary>
    public bool ValidateTokenAudience { get; set; } = true;

    /// <summary>
    /// Gets or sets the clock skew tolerance in minutes.
    /// </summary>
    public int ClockSkewMinutes { get; set; } = 5;

    /// <summary>
    /// Gets or sets the microservice identifier.
    /// </summary>
    public string MicroserviceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the microservice secret.
    /// </summary>
    public string MicroserviceSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of trusted microservices.
    /// </summary>
    public List<string> TrustedMicroservices { get; set; } = new();

    /// <summary>
    /// Gets or sets the Redis connection string for distributed caching.
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the cache key prefix.
    /// </summary>
    public string CacheKeyPrefix { get; set; } = "mamey:distributed:";

    /// <summary>
    /// Gets the JWT validation parameters.
    /// </summary>
    public TokenValidationParameters JwtValidationParameters => new()
    {
        ValidateIssuer = ValidateTokenIssuer,
        ValidateAudience = ValidateTokenAudience,
        ValidateLifetime = ValidateTokenExpiration,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.FromMinutes(ClockSkewMinutes),
        ValidIssuer = JwtIssuer,
        ValidAudience = JwtAudience,
        IssuerSigningKey = GetSigningKey()
    };

    /// <summary>
    /// Gets the signing key for JWT tokens.
    /// </summary>
    /// <returns>The signing key.</returns>
    private SymmetricSecurityKey GetSigningKey()
    {
        if (string.IsNullOrEmpty(JwtSigningKey))
        {
            // Generate a random key if none provided (for development only)
            var key = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            JwtSigningKey = Convert.ToBase64String(key);
        }

        return new SymmetricSecurityKey(Convert.FromBase64String(JwtSigningKey));
    }
}


































