using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Utilities;

namespace Mamey.Identity.Decentralized.Methods.Ethr;

public class DidEthrResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly string _ethrRegistryEndpoint;

    /// <summary>
    /// Create a resolver for did:ethr using the public registry endpoint (e.g., 'https://ethr-did-resolver.identity.org').
    /// </summary>
    public DidEthrResolver(HttpClient httpClient, string ethrRegistryEndpoint)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ethrRegistryEndpoint = ethrRegistryEndpoint?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(ethrRegistryEndpoint));
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (!SupportsMethod(DidUtils.GetMethod(did)))
            throw new NotSupportedException("DID is not did:ethr");

        // Example: https://ethr-did-resolver.identity.org/identifiers/did:ethr:0x....
        var url = $"{_ethrRegistryEndpoint}/identifiers/{did}";
        var resp = await _httpClient.GetAsync(url, cancellationToken);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        var didDocument = DidDocument.Parse(doc.RootElement.GetProperty("didDocument").GetRawText());

        return new DidResolutionResult
        {
            DidDocument = didDocument,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object> { { "resolver", "DidEthrResolver" } }
        };
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "ethr", StringComparison.OrdinalIgnoreCase);
}