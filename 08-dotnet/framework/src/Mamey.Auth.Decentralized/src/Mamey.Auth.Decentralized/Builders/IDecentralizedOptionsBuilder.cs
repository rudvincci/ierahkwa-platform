using Mamey.Auth.Decentralized.Options;

namespace Mamey.Auth.Decentralized.Builders;

/// <summary>
/// Interface for building decentralized authentication options
/// </summary>
public interface IDecentralizedOptionsBuilder
{
    /// <summary>
    /// Sets whether decentralized authentication is enabled
    /// </summary>
    /// <param name="enabled">Whether to enable decentralized authentication</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetEnabled(bool enabled);
    
    /// <summary>
    /// Sets the default DID method
    /// </summary>
    /// <param name="method">The default DID method</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetDefaultMethod(string method);
    
    /// <summary>
    /// Sets the supported DID methods
    /// </summary>
    /// <param name="methods">The supported DID methods</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetSupportedMethods(params string[] methods);
    
    /// <summary>
    /// Sets the Redis cache configuration
    /// </summary>
    /// <param name="connectionString">The Redis connection string</param>
    /// <param name="expiration">The cache expiration time</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetRedisCache(string connectionString, TimeSpan expiration);
    
    /// <summary>
    /// Sets the PostgreSQL configuration
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetPostgreSql(string connectionString);
    
    /// <summary>
    /// Sets the MongoDB configuration
    /// </summary>
    /// <param name="connectionString">The MongoDB connection string</param>
    /// <param name="databaseName">The database name</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetMongoDb(string connectionString, string databaseName);
    
    /// <summary>
    /// Sets the default cryptographic algorithm
    /// </summary>
    /// <param name="algorithm">The default algorithm</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetDefaultCryptoAlgorithm(string algorithm);
    
    /// <summary>
    /// Sets the Verifiable Credentials configuration
    /// </summary>
    /// <param name="enableJwt">Whether to enable VC-JWT</param>
    /// <param name="enableJsonLd">Whether to enable VC JSON-LD</param>
    /// <param name="defaultExpiration">The default VC expiration time</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetVerifiableCredentials(bool enableJwt, bool enableJsonLd, TimeSpan defaultExpiration);
    
    /// <summary>
    /// Sets the issuer and audience
    /// </summary>
    /// <param name="issuer">The issuer DID</param>
    /// <param name="audience">The audience DID</param>
    /// <returns>The options builder</returns>
    IDecentralizedOptionsBuilder SetIssuerAndAudience(string issuer, string audience);
    
    /// <summary>
    /// Builds the decentralized options
    /// </summary>
    /// <returns>The built options</returns>
    DecentralizedOptions Build();
}
