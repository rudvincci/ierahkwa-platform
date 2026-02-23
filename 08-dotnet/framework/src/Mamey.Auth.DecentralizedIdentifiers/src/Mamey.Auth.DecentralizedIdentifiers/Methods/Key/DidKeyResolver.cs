using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Key;

/// <summary>
/// Enhanced DID Key resolver with creation capabilities
/// </summary>
public class DidKeyResolver : IDidResolver
{
    private readonly DidKeyMethod _didKeyMethod;
    private readonly ILogger<DidKeyResolver> _logger;

    public DidKeyResolver(
        DidKeyMethod didKeyMethod,
        ILogger<DidKeyResolver> logger)
    {
        _didKeyMethod = didKeyMethod;
        _logger = logger;
    }
    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!SupportsMethod(DidUtils.GetMethod(did)))
                throw new NotSupportedException("DID is not did:key");

            // Use the enhanced DidKeyMethod to resolve
            var document = await _didKeyMethod.ResolveAsync(did, cancellationToken);
            
            return new DidResolutionResult
            {
                DidDocument = document,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "created", DateTime.UtcNow },
                    { "resolver", "DidKeyResolver" },
                    { "method", "key" }
                },
                ResolutionMetadata = new Dictionary<string, object> 
                { 
                    { "resolver", "DidKeyResolver" },
                    { "method", "key" },
                    { "resolved_at", DateTime.UtcNow }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve did:key: {Did}", did);
            throw;
        }
    }

    /// <summary>
    /// Create a new did:key with generated keys
    /// </summary>
    /// <param name="keyType">Type of key to generate (Ed25519, RSA, etc.)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resolution result with created DID</returns>
    public async Task<DidResolutionResult> CreateAsync(
        string keyType = "Ed25519", 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (did, document, keyPair) = await _didKeyMethod.CreateWithGeneratedKeysAsync(keyType, cancellationToken);
            
            _logger.LogInformation("Created new did:key: {Did}", did);
            
            return new DidResolutionResult
            {
                DidDocument = document,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "created", DateTime.UtcNow },
                    { "resolver", "DidKeyResolver" },
                    { "method", "key" },
                    { "keyType", keyType }
                },
                ResolutionMetadata = new Dictionary<string, object> 
                { 
                    { "resolver", "DidKeyResolver" },
                    { "method", "key" },
                    { "created_at", DateTime.UtcNow },
                    { "keyType", keyType }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create did:key with key type: {KeyType}", keyType);
            throw;
        }
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "key", StringComparison.OrdinalIgnoreCase);
}