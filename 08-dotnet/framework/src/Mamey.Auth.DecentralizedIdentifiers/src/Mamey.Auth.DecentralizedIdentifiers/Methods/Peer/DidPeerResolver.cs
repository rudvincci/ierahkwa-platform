using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Peer;

public class DidPeerResolver : IDidResolver
{
    private readonly ILogger<DidPeerResolver> _logger;

    public DidPeerResolver(ILogger<DidPeerResolver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!SupportsMethod(DidUtils.GetMethod(did)))
                throw new NotSupportedException("DID is not did:peer");

            _logger.LogDebug("Resolving did:peer: {Did}", did);

            var doc = DidDocumentFactory.FromDidPeer(did);
            var numalgo = GetNumalgo(did);

            var result = new DidResolutionResult
            {
                DidDocument = doc,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "resolver", "DidPeerResolver" },
                    { "method", "peer" },
                    { "numalgo", numalgo },
                    { "resolved_at", DateTime.UtcNow }
                },
                ResolutionMetadata = new Dictionary<string, object> 
                { 
                    { "resolver", "DidPeerResolver" },
                    { "method", "peer" },
                    { "numalgo", numalgo },
                    { "resolved_at", DateTime.UtcNow }
                }
            };

            _logger.LogInformation("Successfully resolved did:peer: {Did}, numalgo: {Numalgo}", did, numalgo);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve did:peer: {Did}", did);
            throw;
        }
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "peer", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the numalgo (numbering algorithm) from a peer DID.
    /// </summary>
    private static int GetNumalgo(string did)
    {
        var didObj = new Did(did);
        var msid = didObj.MethodSpecificId;
        
        if (msid.Length > 0 && char.IsDigit(msid[0]))
        {
            return int.Parse(msid[0].ToString());
        }
        
        return -1; // Unknown numalgo
    }
}