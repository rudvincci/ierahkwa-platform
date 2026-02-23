using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Crypto;
using Mamey.Auth.DecentralizedIdentifiers.Methods.MethodBase;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Key;

/// <summary>
/// Implements the did:key method (https://w3c-ccg.github.io/did-method-key/)
/// Enhanced with creation capabilities and full Ed25519/RSA support
/// </summary>
public class DidKeyMethod : DidMethodBase
{
    public override string Name => "key";
    private readonly IDictionary<string, IKeyPairCryptoProvider> _cryptoProviders;
    private readonly ILogger<DidKeyMethod> _logger;

    /// <summary>
    /// Accepts a mapping of supported cryptography providers.
    /// </summary>
    public DidKeyMethod(
        IEnumerable<IKeyPairCryptoProvider> cryptoProviders,
        ILogger<DidKeyMethod> logger)
    {
        _cryptoProviders = cryptoProviders.ToDictionary(x => x.KeyType, StringComparer.OrdinalIgnoreCase);
        _logger = logger;
    }

    /// <summary>
    /// Create a new did:key from a key pair
    /// </summary>
    /// <param name="keyType">Type of key (Ed25519, RSA, etc.)</param>
    /// <param name="publicKey">Public key bytes</param>
    /// <param name="privateKey">Private key bytes (optional, for signing)</param>
    /// <returns>Created DID and DID document</returns>
    public async Task<(string Did, IDidDocument Document)> CreateAsync(
        string keyType, 
        byte[] publicKey, 
        byte[] privateKey = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_cryptoProviders.TryGetValue(keyType, out var provider))
                throw new NotSupportedException($"Key type '{keyType}' is not supported.");

            // Generate multicodec encoded key
            var multicodecBytes = MulticodecUtil.EncodeKey(keyType, publicKey);
            var multibase = MultibaseUtil.Encode(multicodecBytes, MultibaseUtil.Base.Base58Btc);
            
            // Create DID
            var did = $"did:key:{multibase}";
            
            // Create DID document
            var document = await CreateDidDocument(did, keyType, publicKey, provider);
            
            _logger.LogInformation("Created did:key: {Did}", did);
            
            return (did, document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create did:key with key type: {KeyType}", keyType);
            throw;
        }
    }

    /// <summary>
    /// Create a new did:key with generated key pair
    /// </summary>
    /// <param name="keyType">Type of key to generate (Ed25519, RSA, etc.)</param>
    /// <returns>Created DID, DID document, and key pair</returns>
    public async Task<(string Did, IDidDocument Document, (byte[] PrivateKey, byte[] PublicKey) KeyPair)> CreateWithGeneratedKeysAsync(
        string keyType,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_cryptoProviders.TryGetValue(keyType, out var provider))
                throw new NotSupportedException($"Key type '{keyType}' is not supported.");

            // Generate key pair
            var (privateKey, publicKey) = await provider.GenerateKeyPairAsync(keyType);
            
            // Create DID
            var result = await CreateAsync(keyType, publicKey, privateKey, cancellationToken);
            var did = result.Did;
            var document = result.Document;
            
            _logger.LogInformation("Created did:key with generated keys: {Did}", did);
            
            return (did, document, (privateKey, publicKey));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create did:key with generated keys for type: {KeyType}", keyType);
            throw;
        }
    }

    /// <summary>
    /// Resolve a did:key to its DID document
    /// </summary>
    public override async Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        // Parse the multibase/multicodec encoded public key from the method-specific id
        var didObj = new Did(did);
        var methodSpecificId = didObj.MethodSpecificId;

        // e.g., z6Mki... (multibase, multicodec)
        var (keyType, publicKeyBytes, codec) = MulticodecKeyParser.Parse(methodSpecificId);
        if (!_cryptoProviders.TryGetValue(keyType, out var provider))
            throw new NotSupportedException($"Key type '{keyType}' is not supported by this implementation.");

        var publicKeyMultibase = "z" + methodSpecificId;
        var vmId = $"{did}#key-1";
        var vmType = provider.VerificationMethodType;

        var verificationMethod = new VerificationMethod(
            id: vmId,
            type: vmType,
            controller: did,
            publicKeyJwk: provider.ExportJwk(publicKeyBytes),
            publicKeyBase58: provider.ExportBase58(publicKeyBytes),
            publicKeyMultibase: publicKeyMultibase);

        var doc = new DidDocument(
            context: new[] { DidContextConstants.W3cDidContext },
            id: did,
            controller: new[] { did },
            verificationMethods: new[] { verificationMethod },
            authentication: new[] { vmId }
        );
        return await Task.FromResult(doc);
    }

    /// <summary>
    /// Create a DID document for a did:key
    /// </summary>
    private async Task<IDidDocument> CreateDidDocument(
        string did, 
        string keyType, 
        byte[] publicKey, 
        IKeyPairCryptoProvider provider)
    {
        try
        {
            // Generate multicodec encoded key for verification method
            var multicodecBytes = MulticodecUtil.EncodeKey(keyType, publicKey);
            var multibase = MultibaseUtil.Encode(multicodecBytes, MultibaseUtil.Base.Base58Btc);
            
            var publicKeyMultibase = "z" + multibase;
            var vmId = $"{did}#key-1";
            var vmType = provider.VerificationMethodType;

            var verificationMethod = new VerificationMethod(
                id: vmId,
                type: vmType,
                controller: did,
                publicKeyJwk: provider.ExportJwk(publicKey),
                publicKeyBase58: provider.ExportBase58(publicKey),
                publicKeyMultibase: publicKeyMultibase);

            var doc = new DidDocument(
                context: new[] { DidContextConstants.W3cDidContext },
                id: did,
                controller: new[] { did },
                verificationMethods: new[] { verificationMethod },
                authentication: new[] { vmId }
            );

            return await Task.FromResult(doc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create DID document for did:key: {Did}", did);
            throw;
        }
    }

    /// <summary>
    /// Validate that a DID is a valid did:key
    /// </summary>
    private void ValidateDid(string did)
    {
        if (string.IsNullOrEmpty(did))
            throw new ArgumentException("DID cannot be null or empty");

        if (!did.StartsWith("did:key:"))
            throw new ArgumentException("DID must be a did:key");

        var methodSpecificId = did.Substring("did:key:".Length);
        if (string.IsNullOrEmpty(methodSpecificId))
            throw new ArgumentException("DID method-specific ID cannot be empty");

        if (!methodSpecificId.StartsWith("z"))
            throw new ArgumentException("DID method-specific ID must be multibase encoded (start with 'z')");
    }
}