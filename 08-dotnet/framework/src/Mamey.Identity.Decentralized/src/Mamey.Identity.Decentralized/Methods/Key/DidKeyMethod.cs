using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Crypto;
using Mamey.Identity.Decentralized.Methods.MethodBase;

namespace Mamey.Identity.Decentralized.Methods.Key;

/// <summary>
/// Implements the did:key method (https://w3c-ccg.github.io/did-method-key/)
/// </summary>
public class DidKeyMethod : DidMethodBase
{
    public override string Name => "key";
    private readonly IDictionary<string, IKeyPairCryptoProvider> _cryptoProviders;

    /// <summary>
    /// Accepts a mapping of supported cryptography providers.
    /// </summary>
    public DidKeyMethod(IEnumerable<IKeyPairCryptoProvider> cryptoProviders)
    {
        _cryptoProviders = cryptoProviders.ToDictionary(x => x.KeyType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// did:key does not support create/update/deactivate, only self-certifying resolution.
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
}