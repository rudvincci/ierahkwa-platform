using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Utilities;

namespace Mamey.Identity.Decentralized.Methods.Peer;

public class DidPeerResolver : IDidResolver
{
    public Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (!SupportsMethod(DidUtils.GetMethod(did)))
            throw new NotSupportedException("DID is not did:peer");

        // Only Numalgo 0/1 supported in this example (see your previous code)
        var doc = DidDocumentFactory.FromDidPeer(did); // Utility helper you need to implement

        return Task.FromResult(new DidResolutionResult
        {
            DidDocument = doc,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object> { { "resolver", "DidPeerResolver" } }
        });
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "peer", StringComparison.OrdinalIgnoreCase);
}