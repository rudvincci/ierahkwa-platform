using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Mamey.Identity.Decentralized.Utilities;

namespace Mamey.Identity.Decentralized.Methods.Ion;

public class DidIonResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly string _ionApiBaseUrl;

    public DidIonResolver(HttpClient httpClient, string ionApiBaseUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ionApiBaseUrl = ionApiBaseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(ionApiBaseUrl));
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        if (!SupportsMethod(DidUtils.GetMethod(did)))
            throw new NotSupportedException("DID is not did:ion");

        var url = $"{_ionApiBaseUrl}/identifiers/{did}";
        var resp = await _httpClient.GetAsync(url, cancellationToken);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(json);

        var didDocument = DidDocument.Parse(doc.RootElement.GetProperty("didDocument").GetRawText());

        return new DidResolutionResult
        {
            DidDocument = didDocument,
            DocumentMetadata = new System.Collections.Generic.Dictionary<string, object>(),
            ResolutionMetadata = new System.Collections.Generic.Dictionary<string, object> { { "resolver", "DidIonResolver" } }
        };
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "ion", StringComparison.OrdinalIgnoreCase);
}