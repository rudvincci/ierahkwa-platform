using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;

namespace Mamey.Identity.Decentralized.Resolution;

/// <summary>
/// Dereferences DID URLs according to the W3C DID URL Dereferencing spec, including fragment, key, and service lookups.
/// </summary>
public class Dereferencer : IDidDereferencer
{
    private readonly IDidResolver _resolver;

    /// <summary>
    /// Constructs a Dereferencer with the given resolver.
    /// </summary>
    public Dereferencer(IDidResolver resolver)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    /// <inheritdoc />
    public async Task<DidDereferencingResult> DereferenceAsync(string didUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(didUrl))
            throw new ArgumentNullException(nameof(didUrl));

        var url = new DidUrl(didUrl);
        var result = await _resolver.ResolveAsync(url.Did.ToString(), cancellationToken);
        if (result.DidDocument == null)
            throw new Exception($"DID Document not found for {url.Did}");

        object dereferenced = result.DidDocument;
        string contentType = "application/did+ld+json";

        // If a fragment is specified, locate the referenced key or service
        if (!string.IsNullOrWhiteSpace(url.Fragment))
        {
            var fragmentId = url.Did + "#" + url.Fragment;

            // Search verification methods
            var vm = result.DidDocument.VerificationMethods?.FirstOrDefault(v => v.Id == fragmentId);
            if (vm != null)
            {
                dereferenced = vm;
                contentType = "application/ld+json";
            }
            else
            {
                // Search service endpoints
                var svc = result.DidDocument.ServiceEndpoints?.FirstOrDefault(s => s.Id == fragmentId);
                if (svc != null)
                {
                    dereferenced = svc;
                    contentType = "application/ld+json";
                }
                else
                {
                    throw new Exception($"Fragment '{url.Fragment}' not found in {url.Did}");
                }
            }
        }

        return new DidDereferencingResult
        {
            Content = dereferenced,
            ContentType = contentType,
            DereferencingMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = result.ResolutionMetadata
        };
    }
}