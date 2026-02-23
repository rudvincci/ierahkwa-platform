using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Methods.Peer;
using Mamey.Identity.Decentralized.Utilities;

namespace Mamey.Identity.Decentralized.Methods.Key;

public class DidKeyResolver : IDidResolver
{
    public Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (!SupportsMethod(DidUtils.GetMethod(did)))
            throw new NotSupportedException("DID is not did:key");

        // The method-specific-id is the key (multibase/multicodec)
        var doc = DidDocumentFactory.FromDidKey(did); // You need a helper for did:key doc construction (see below)
        return Task.FromResult(new DidResolutionResult
        {
            DidDocument = doc,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object> { { "resolver", "DidKeyResolver" } }
        });
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "key", StringComparison.OrdinalIgnoreCase);
}