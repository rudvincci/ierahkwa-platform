using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Mamey.Auth.DecentralizedIdentifiers.Caching;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ethr;

/// <summary>
/// Enhanced DID Ethr resolver with caching, gas optimization, and better error handling
/// </summary>
public class DidEthrResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly string _ethrRegistryEndpoint;
    private readonly IDidDocumentCache _cache;
    private readonly ILogger<DidEthrResolver> _logger;
    private readonly EthrResolverOptions _options;

    /// <summary>
    /// Create a resolver for did:ethr using the public registry endpoint (e.g., 'https://ethr-did-resolver.identity.org').
    /// </summary>
    public DidEthrResolver(
        HttpClient httpClient, 
        string ethrRegistryEndpoint,
        IDidDocumentCache cache,
        ILogger<DidEthrResolver> logger,
        EthrResolverOptions options = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ethrRegistryEndpoint = ethrRegistryEndpoint?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(ethrRegistryEndpoint));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? new EthrResolverOptions();
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!SupportsMethod(DidUtils.GetMethod(did)))
                throw new NotSupportedException("DID is not did:ethr");

            // Check cache first
            if (_options.EnableCaching)
            {
                var cachedDoc = await _cache.GetAsync(did);
                if (cachedDoc != null)
                {
                    _logger.LogDebug("Resolved did:ethr from cache: {Did}", did);
                    return new DidResolutionResult
                    {
                        DidDocument = cachedDoc,
                        DocumentMetadata = new Dictionary<string, object>
                        {
                            { "cached", true },
                            { "resolver", "DidEthrResolver" }
                        },
                        ResolutionMetadata = new Dictionary<string, object>
                        {
                            { "resolver", "DidEthrResolver" },
                            { "method", "ethr" },
                            { "cached", true },
                            { "resolved_at", DateTime.UtcNow }
                        }
                    };
                }
            }

            // Resolve from Ethereum registry
            var url = $"{_ethrRegistryEndpoint}/identifiers/{did}";
            _logger.LogDebug("Resolving did:ethr from URL: {Url}", url);

            var resp = await _httpClient.GetAsync(url, cancellationToken);
            
            if (resp.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException($"DID document not found for {did}");
            }
            
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            if (!doc.RootElement.TryGetProperty("didDocument", out var didDocumentElement))
            {
                throw new InvalidOperationException($"Invalid response format from {url}");
            }

            var didDocument = DidDocument.Parse(didDocumentElement.GetRawText());

            if (didDocument == null)
            {
                throw new InvalidOperationException($"Failed to parse DID document for {did}");
            }

            // Validate DID document
            ValidateDidDocument(didDocument, did);

            // Cache the result
            if (_options.EnableCaching)
            {
                await _cache.SetAsync(did, didDocument, _options.CacheTtlMinutes);
            }

            _logger.LogInformation("Successfully resolved did:ethr: {Did}", did);

            return new DidResolutionResult
            {
                DidDocument = didDocument,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "cached", false },
                    { "resolver", "DidEthrResolver" },
                    { "registry_endpoint", _ethrRegistryEndpoint },
                    { "resolved_at", DateTime.UtcNow }
                },
                ResolutionMetadata = new Dictionary<string, object>
                {
                    { "resolver", "DidEthrResolver" },
                    { "method", "ethr" },
                    { "registry_endpoint", _ethrRegistryEndpoint },
                    { "resolved_at", DateTime.UtcNow }
                }
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error resolving did:ethr: {Did}", did);
            throw new InvalidOperationException($"Failed to resolve did:ethr: {did}", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout resolving did:ethr: {Did}", did);
            throw new InvalidOperationException($"Timeout resolving did:ethr: {did}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving did:ethr: {Did}", did);
            throw;
        }
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "ethr", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Validate DID document structure and content
    /// </summary>
    private void ValidateDidDocument(DidDocument doc, string expectedDid)
    {
        if (string.IsNullOrEmpty(doc.Id))
            throw new InvalidOperationException("DID document missing ID");

        if (doc.Id != expectedDid)
            throw new InvalidOperationException($"DID document ID mismatch. Expected: {expectedDid}, Found: {doc.Id}");

        if (doc.VerificationMethods == null || !doc.VerificationMethods.Any())
            throw new InvalidOperationException("DID document missing verification methods");

        if (doc.Context == null || !doc.Context.Any())
            throw new InvalidOperationException("DID document missing context");

        _logger.LogDebug("DID document validation passed for: {Did}", expectedDid);
    }
}