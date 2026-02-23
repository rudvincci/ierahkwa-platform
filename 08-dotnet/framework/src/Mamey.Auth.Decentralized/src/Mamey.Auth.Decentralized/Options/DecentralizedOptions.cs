using System.ComponentModel.DataAnnotations;

namespace Mamey.Auth.Decentralized.Options;

/// <summary>
/// Configuration options for decentralized authentication.
/// </summary>
public class DecentralizedOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether DID authentication is enabled.
    /// </summary>
    public bool EnableDidAuthentication { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether DID validation is enabled.
    /// </summary>
    public bool EnableDidValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether caching is enabled.
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the custom DID header.
    /// </summary>
    public string DidHeaderName { get; set; } = "X-DID";

    /// <summary>
    /// Gets or sets the default cache duration.
    /// </summary>
    public TimeSpan DefaultCacheDuration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets the DID document cache duration.
    /// </summary>
    public TimeSpan DidDocumentCacheDuration { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the verification method cache duration.
    /// </summary>
    public TimeSpan VerificationMethodCacheDuration { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Gets or sets the service endpoint cache duration.
    /// </summary>
    public TimeSpan ServiceEndpointCacheDuration { get; set; } = TimeSpan.FromHours(2);

    /// <summary>
    /// Gets or sets the maximum cache size.
    /// </summary>
    public int MaxCacheSize { get; set; } = 10000;

    /// <summary>
    /// Gets or sets the cache cleanup interval.
    /// </summary>
    public TimeSpan CacheCleanupInterval { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the supported DID methods.
    /// </summary>
    public List<string> SupportedDidMethods { get; set; } = new() { "web", "key" };

    /// <summary>
    /// Gets or sets the supported verification method types.
    /// </summary>
    public List<string> SupportedVerificationMethodTypes { get; set; } = new()
    {
        "Ed25519VerificationKey2020",
        "RsaVerificationKey2018",
        "Secp256k1VerificationKey2018"
    };

    /// <summary>
    /// Gets or sets the supported service types.
    /// </summary>
    public List<string> SupportedServiceTypes { get; set; } = new()
    {
        "DIDCommMessaging",
        "LinkedDomains",
        "DIDCommV2"
    };

    /// <summary>
    /// Gets or sets the maximum DID length.
    /// </summary>
    [Range(1, 2048)]
    public int MaxDidLength { get; set; } = 2048;

    /// <summary>
    /// Gets or sets the maximum DID document size in bytes.
    /// </summary>
    [Range(1024, 1048576)] // 1KB to 1MB
    public int MaxDidDocumentSize { get; set; } = 1048576;

    /// <summary>
    /// Gets or sets the maximum verification methods per DID document.
    /// </summary>
    [Range(1, 100)]
    public int MaxVerificationMethods { get; set; } = 100;

    /// <summary>
    /// Gets or sets the maximum service endpoints per DID document.
    /// </summary>
    [Range(1, 100)]
    public int MaxServiceEndpoints { get; set; } = 100;

    /// <summary>
    /// Gets or sets the timeout for DID resolution.
    /// </summary>
    public TimeSpan ResolutionTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the timeout for verification method validation.
    /// </summary>
    public TimeSpan ValidationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Gets or sets the retry count for failed operations.
    /// </summary>
    [Range(0, 5)]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Gets or sets the retry delay.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets a value indicating whether to enable detailed logging.
    /// </summary>
    public bool EnableDetailedLogging { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to enable performance metrics.
    /// </summary>
    public bool EnablePerformanceMetrics { get; set; } = true;

    /// <summary>
    /// Gets or sets the performance metrics collection interval.
    /// </summary>
    public TimeSpan MetricsCollectionInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the health check timeout.
    /// </summary>
    public TimeSpan HealthCheckTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Gets or sets the health check interval.
    /// </summary>
    public TimeSpan HealthCheckInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the circuit breaker failure threshold.
    /// </summary>
    [Range(1, 100)]
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Gets or sets the circuit breaker timeout.
    /// </summary>
    public TimeSpan CircuitBreakerTimeout { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the circuit breaker retry timeout.
    /// </summary>
    public TimeSpan CircuitBreakerRetryTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the rate limiting requests per minute.
    /// </summary>
    [Range(1, 10000)]
    public int RateLimitRequestsPerMinute { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the rate limiting burst size.
    /// </summary>
    [Range(1, 1000)]
    public int RateLimitBurstSize { get; set; } = 100;

    /// <summary>
    /// Gets or sets the rate limiting window size.
    /// </summary>
    public TimeSpan RateLimitWindowSize { get; set; } = TimeSpan.FromMinutes(1);

    // Additional properties for builder compatibility
    /// <summary>
    /// Gets or sets whether decentralized authentication is enabled (alias for EnableDidAuthentication).
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default DID method.
    /// </summary>
    public string DefaultMethod { get; set; } = "web";

    /// <summary>
    /// Gets or sets the supported DID methods (alias for SupportedDidMethods).
    /// </summary>
    public List<string> SupportedMethods { get; set; } = new() { "web", "key" };

    /// <summary>
    /// Gets or sets whether to use Redis cache.
    /// </summary>
    public bool UseRedisCache { get; set; } = true;

    /// <summary>
    /// Gets or sets the Redis cache connection string.
    /// </summary>
    public string RedisCacheConnection { get; set; } = "localhost:6379";

    /// <summary>
    /// Gets or sets the cache expiration time.
    /// </summary>
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets whether to use PostgreSQL store.
    /// </summary>
    public bool UsePostgreSqlStore { get; set; } = true;

    /// <summary>
    /// Gets or sets the PostgreSQL connection string.
    /// </summary>
    public string PostgreSqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to use MongoDB store.
    /// </summary>
    public bool UseMongoStore { get; set; } = true;

    /// <summary>
    /// Gets or sets the MongoDB connection string.
    /// </summary>
    public string MongoConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// Gets or sets the MongoDB database name.
    /// </summary>
    public string MongoDatabaseName { get; set; } = "decentralized_auth";

    /// <summary>
    /// Gets or sets the default cryptographic algorithm.
    /// </summary>
    public string DefaultCryptoAlgorithm { get; set; } = "Ed25519";

    /// <summary>
    /// Gets or sets whether to enable VC-JWT.
    /// </summary>
    public bool EnableVcJwt { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable VC JSON-LD.
    /// </summary>
    public bool EnableVcJsonLd { get; set; } = true;

    /// <summary>
    /// Gets or sets the default VC expiration time.
    /// </summary>
    public TimeSpan DefaultVcExpiration { get; set; } = TimeSpan.FromDays(365);

    /// <summary>
    /// Gets or sets the issuer DID.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the audience DID.
    /// </summary>
    public string Audience { get; set; } = string.Empty;
}