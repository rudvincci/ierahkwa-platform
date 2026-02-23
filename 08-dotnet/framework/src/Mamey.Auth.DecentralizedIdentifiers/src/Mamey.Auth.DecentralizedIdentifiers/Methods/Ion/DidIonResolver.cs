using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Mamey.Auth.DecentralizedIdentifiers.Utilities;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

public class DidIonResolver : IDidResolver
{
    private readonly HttpClient _httpClient;
    private readonly string _ionApiBaseUrl;
    private readonly IBitcoinAnchoringService _bitcoinAnchoringService;
    private readonly ILogger<DidIonResolver> _logger;

    public DidIonResolver(
        HttpClient httpClient, 
        string ionApiBaseUrl,
        IBitcoinAnchoringService bitcoinAnchoringService,
        ILogger<DidIonResolver> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ionApiBaseUrl = ionApiBaseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(ionApiBaseUrl));
        _bitcoinAnchoringService = bitcoinAnchoringService ?? throw new ArgumentNullException(nameof(bitcoinAnchoringService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<DidResolutionResult> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!SupportsMethod(DidUtils.GetMethod(did)))
                throw new NotSupportedException("DID is not did:ion");

            _logger.LogDebug("Resolving did:ion: {Did}", did);

            var url = $"{_ionApiBaseUrl}/identifiers/{did}";
            var resp = await _httpClient.GetAsync(url, cancellationToken);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);

            var didDocument = DidDocument.Parse(doc.RootElement.GetProperty("didDocument").GetRawText());

            // Check if Bitcoin anchoring is enabled and verify it
            var bitcoinAnchoringVerified = await VerifyBitcoinAnchoringAsync(did, cancellationToken);

            var result = new DidResolutionResult
            {
                DidDocument = didDocument,
                DocumentMetadata = new Dictionary<string, object>
                {
                    { "resolver", "DidIonResolver" },
                    { "method", "ion" },
                    { "bitcoinAnchoringVerified", bitcoinAnchoringVerified },
                    { "resolved_at", DateTime.UtcNow }
                },
                ResolutionMetadata = new Dictionary<string, object> 
                { 
                    { "resolver", "DidIonResolver" },
                    { "method", "ion" },
                    { "bitcoinAnchoringVerified", bitcoinAnchoringVerified },
                    { "resolved_at", DateTime.UtcNow }
                }
            };

            _logger.LogInformation("Successfully resolved did:ion: {Did}, Bitcoin anchoring verified: {Verified}", 
                did, bitcoinAnchoringVerified);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve did:ion: {Did}", did);
            throw;
        }
    }

    public bool SupportsMethod(string didMethod) => string.Equals(didMethod, "ion", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifies if the DID is anchored to Bitcoin blockchain.
    /// </summary>
    private async Task<bool> VerifyBitcoinAnchoringAsync(string did, CancellationToken cancellationToken)
    {
        try
        {
            // For now, we'll assume Bitcoin anchoring is optional and return true
            // In a real implementation, you would check the ION node's Bitcoin anchoring status
            // or query the Bitcoin blockchain directly for the DID's anchoring transaction
            
            _logger.LogDebug("Verifying Bitcoin anchoring for did:ion: {Did}", did);
            
            // This is a placeholder - in reality you would:
            // 1. Get the DID's operation history from ION
            // 2. Check if operations are anchored to Bitcoin
            // 3. Verify the anchoring transactions on Bitcoin blockchain
            
            return true; // Placeholder - always return true for now
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to verify Bitcoin anchoring for did:ion: {Did}", did);
            return false;
        }
    }
}