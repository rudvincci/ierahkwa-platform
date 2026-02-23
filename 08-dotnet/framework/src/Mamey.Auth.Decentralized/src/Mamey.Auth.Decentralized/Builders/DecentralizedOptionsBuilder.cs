using Mamey.Auth.Decentralized.Options;

namespace Mamey.Auth.Decentralized.Builders;

/// <summary>
/// Builder for decentralized authentication options
/// </summary>
public class DecentralizedOptionsBuilder : IDecentralizedOptionsBuilder
{
    private readonly DecentralizedOptions _options = new();
    
    /// <summary>
    /// Sets whether decentralized authentication is enabled
    /// </summary>
    /// <param name="enabled">Whether to enable decentralized authentication</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetEnabled(bool enabled)
    {
        _options.Enabled = enabled;
        return this;
    }
    
    /// <summary>
    /// Sets the default DID method
    /// </summary>
    /// <param name="method">The default DID method</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetDefaultMethod(string method)
    {
        if (string.IsNullOrEmpty(method))
            throw new ArgumentException("Method cannot be null or empty", nameof(method));
        
        _options.DefaultMethod = method;
        return this;
    }
    
    /// <summary>
    /// Sets the supported DID methods
    /// </summary>
    /// <param name="methods">The supported DID methods</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetSupportedMethods(params string[] methods)
    {
        if (methods == null)
            throw new ArgumentNullException(nameof(methods));
        
        if (methods.Length == 0)
            throw new ArgumentException("At least one method must be specified", nameof(methods));
        
        _options.SupportedMethods = methods.ToList();
        return this;
    }
    
    /// <summary>
    /// Sets the Redis cache configuration
    /// </summary>
    /// <param name="connectionString">The Redis connection string</param>
    /// <param name="expiration">The cache expiration time</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetRedisCache(string connectionString, TimeSpan expiration)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        
        _options.UseRedisCache = true;
        _options.RedisCacheConnection = connectionString;
        _options.CacheExpiration = expiration;
        return this;
    }
    
    /// <summary>
    /// Sets the PostgreSQL configuration
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetPostgreSql(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        
        _options.UsePostgreSqlStore = true;
        _options.PostgreSqlConnectionString = connectionString;
        return this;
    }
    
    /// <summary>
    /// Sets the MongoDB configuration
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string</param>
    /// <param name="databaseName">The database name</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetMongoDb(string connectionString, string databaseName)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string cannot be null or empty", nameof(connectionString));
        
        if (string.IsNullOrEmpty(databaseName))
            throw new ArgumentException("Database name cannot be null or empty", nameof(databaseName));
        
        _options.UseMongoStore = true;
        _options.MongoConnectionString = connectionString;
        _options.MongoDatabaseName = databaseName;
        return this;
    }
    
    /// <summary>
    /// Sets the default cryptographic algorithm
    /// </summary>
    /// <param name="algorithm">The default algorithm</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetDefaultCryptoAlgorithm(string algorithm)
    {
        if (string.IsNullOrEmpty(algorithm))
            throw new ArgumentException("Algorithm cannot be null or empty", nameof(algorithm));
        
        _options.DefaultCryptoAlgorithm = algorithm;
        return this;
    }
    
    /// <summary>
    /// Sets the Verifiable Credentials configuration
    /// </summary>
    /// <param name="enableJwt">Whether to enable VC-JWT</param>
    /// <param name="enableJsonLd">Whether to enable VC JSON-LD</param>
    /// <param name="defaultExpiration">The default VC expiration time</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetVerifiableCredentials(bool enableJwt, bool enableJsonLd, TimeSpan defaultExpiration)
    {
        _options.EnableVcJwt = enableJwt;
        _options.EnableVcJsonLd = enableJsonLd;
        _options.DefaultVcExpiration = defaultExpiration;
        return this;
    }
    
    /// <summary>
    /// Sets the issuer and audience
    /// </summary>
    /// <param name="issuer">The issuer DID</param>
    /// <param name="audience">The audience DID</param>
    /// <returns>The options builder</returns>
    public IDecentralizedOptionsBuilder SetIssuerAndAudience(string issuer, string audience)
    {
        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentException("Issuer cannot be null or empty", nameof(issuer));
        
        if (string.IsNullOrEmpty(audience))
            throw new ArgumentException("Audience cannot be null or empty", nameof(audience));
        
        _options.Issuer = issuer;
        _options.Audience = audience;
        return this;
    }
    
    /// <summary>
    /// Builds the decentralized options
    /// </summary>
    /// <returns>The built options</returns>
    public DecentralizedOptions Build()
    {
        // Validate the options
        ValidateOptions();
        
        return _options;
    }
    
    /// <summary>
    /// Validates the built options
    /// </summary>
    private void ValidateOptions()
    {
        if (!_options.Enabled)
            return;
        
        if (string.IsNullOrEmpty(_options.DefaultMethod))
            throw new InvalidOperationException("Default method must be specified when enabled");
        
        if (!_options.SupportedMethods.Contains(_options.DefaultMethod))
            throw new InvalidOperationException("Default method must be in the list of supported methods");
        
        if (_options.UseRedisCache && string.IsNullOrEmpty(_options.RedisCacheConnection))
            throw new InvalidOperationException("Redis connection string must be specified when Redis cache is enabled");
        
        if (_options.UsePostgreSqlStore && string.IsNullOrEmpty(_options.PostgreSqlConnectionString))
            throw new InvalidOperationException("PostgreSQL connection string must be specified when PostgreSQL store is enabled");
        
        if (_options.UseMongoStore && string.IsNullOrEmpty(_options.MongoConnectionString))
            throw new InvalidOperationException("MongoDB connection string must be specified when MongoDB store is enabled");
        
        if (string.IsNullOrEmpty(_options.DefaultCryptoAlgorithm))
            throw new InvalidOperationException("Default crypto algorithm must be specified");
        
        if (_options.EnableVcJwt && string.IsNullOrEmpty(_options.Issuer))
            throw new InvalidOperationException("Issuer must be specified when VC-JWT is enabled");
        
        if (_options.EnableVcJsonLd && string.IsNullOrEmpty(_options.Issuer))
            throw new InvalidOperationException("Issuer must be specified when VC JSON-LD is enabled");
    }
}
