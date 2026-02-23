using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Methods.MethodBase;
using Microsoft.Extensions.Logging;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ion;

/// <summary>
/// Implements the did:ion method (https://identity.foundation/ion/)
/// : resolves, creates, updates, and deactivates ION DIDs.
/// </summary>
public class DidIonMethod : DidMethodBase
{
    public override string Name => "ion";
    private readonly HttpClient _httpClient;
    private readonly string _ionNodeApiBaseUrl; // e.g., "https://ion.tbddev.org"
    private readonly IBitcoinAnchoringService _bitcoinAnchoringService;
    private readonly ILogger<DidIonMethod> _logger;

    /// <summary>
    /// Constructs an ION method handler using the specified ION node or API base URL.
    /// </summary>
    public DidIonMethod(
        HttpClient httpClient, 
        string ionNodeApiBaseUrl,
        IBitcoinAnchoringService bitcoinAnchoringService,
        ILogger<DidIonMethod> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ionNodeApiBaseUrl = ionNodeApiBaseUrl?.TrimEnd('/') ??
                             throw new ArgumentNullException(nameof(ionNodeApiBaseUrl));
        _bitcoinAnchoringService = bitcoinAnchoringService ?? throw new ArgumentNullException(nameof(bitcoinAnchoringService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Resolves a did:ion DID Document from the ION node API.
    /// </summary>
    public override async Task<IDidDocument> ResolveAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        var url = $"{_ionNodeApiBaseUrl}/identifiers/{did}";
        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var ionResponse =
            await JsonSerializer.DeserializeAsync<IonResolveResponse>(stream, cancellationToken: cancellationToken);

        if (ionResponse?.DidDocument == null)
            throw new Exception($"DID Document not found in ION response for {did}.");

        return ionResponse.DidDocument;
    }

    /// <summary>
    /// Creates a new did:ion by submitting a create operation to the ION node.
    /// </summary>
    public override async Task<IDidDocument> CreateAsync(object options, CancellationToken cancellationToken = default)
    {
        if (options is not IonMethodOptions ionOptions)
            throw new ArgumentException("Invalid options for did:ion creation.");

        try
        {
            _logger.LogInformation("Creating did:ion with Bitcoin anchoring: {Enabled}", ionOptions.BitcoinAnchoring?.Enabled);

            // Build ION "create" request
            var createRequest = IonOperationBuilder.BuildCreateOperation(ionOptions);
            var json = JsonSerializer.Serialize(createRequest, new JsonSerializerOptions { WriteIndented = true });

            var url = $"{_ionNodeApiBaseUrl}/operations";
            var response = await _httpClient.PostAsync(url, new StringContent(json), cancellationToken);
            response.EnsureSuccessStatusCode();

            // ION does not immediately resolve new DIDs (blockchain latency),
            // so you may need to poll for full resolution or return a partial DID document
            // For demo, just extract short-form DID from response if available
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var opResponse = JsonSerializer.Deserialize<IonCreateResponse>(content);

            if (opResponse == null || string.IsNullOrWhiteSpace(opResponse.Did))
                throw new Exception("ION create response missing DID.");

            // If Bitcoin anchoring is enabled, anchor the operation
            if (ionOptions.BitcoinAnchoring?.Enabled == true)
            {
                var operationHash = ComputeOperationHash(createRequest);
                var anchoringResult = await _bitcoinAnchoringService.AnchorToBitcoinAsync(
                    operationHash, 
                    ionOptions.BitcoinAnchoring, 
                    cancellationToken);

                if (!anchoringResult.Success)
                {
                    _logger.LogWarning("Bitcoin anchoring failed for DID {Did}: {Error}", 
                        opResponse.Did, anchoringResult.ErrorMessage);
                }
                else
                {
                    _logger.LogInformation("Successfully anchored DID {Did} to Bitcoin: {TransactionId}", 
                        opResponse.Did, anchoringResult.TransactionId);
                }
            }

            // Poll for DID resolution or return as-is
            return await ResolveAsync(opResponse.Did, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create did:ion");
            throw;
        }
    }

    /// <summary>
    /// Updates a did:ion DID by submitting an update operation.
    /// </summary>
    public override async Task<IDidDocument> UpdateAsync(string did, object updateRequest,
        CancellationToken cancellationToken = default)
    {
        ValidateDid(did);
        if (updateRequest is not IonUpdateOptions updateOptions)
            throw new ArgumentException("updateRequest must be IonUpdateOptions.");

        try
        {
            _logger.LogInformation("Updating did:ion: {Did}", did);

            var updateOp = IonOperationBuilder.BuildUpdateOperation(did, updateOptions);
            var json = JsonSerializer.Serialize(updateOp, new JsonSerializerOptions { WriteIndented = true });

            var url = $"{_ionNodeApiBaseUrl}/operations";
            var response = await _httpClient.PostAsync(url, new StringContent(json), cancellationToken);
            response.EnsureSuccessStatusCode();

            // If Bitcoin anchoring is enabled, anchor the update operation
            if (updateOptions.BitcoinAnchoring?.Enabled == true)
            {
                var operationHash = ComputeOperationHash(updateOp);
                var anchoringResult = await _bitcoinAnchoringService.AnchorToBitcoinAsync(
                    operationHash, 
                    updateOptions.BitcoinAnchoring, 
                    cancellationToken);

                if (!anchoringResult.Success)
                {
                    _logger.LogWarning("Bitcoin anchoring failed for DID update {Did}: {Error}", 
                        did, anchoringResult.ErrorMessage);
                }
                else
                {
                    _logger.LogInformation("Successfully anchored DID update {Did} to Bitcoin: {TransactionId}", 
                        did, anchoringResult.TransactionId);
                }
            }

            // Poll for new state or re-resolve
            return await ResolveAsync(did, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update did:ion: {Did}", did);
            throw;
        }
    }

    /// <summary>
    /// Deactivates a did:ion by submitting a deactivate operation.
    /// </summary>
    public override async Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        try
        {
            _logger.LogInformation("Deactivating did:ion: {Did}", did);

            var deactivateOp = IonOperationBuilder.BuildDeactivateOperation(did);
            var json = JsonSerializer.Serialize(deactivateOp, new JsonSerializerOptions { WriteIndented = true });

            var url = $"{_ionNodeApiBaseUrl}/operations";
            var response = await _httpClient.PostAsync(url, new StringContent(json), cancellationToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Successfully deactivated did:ion: {Did}", did);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate did:ion: {Did}", did);
            throw;
        }
    }

    /// <summary>
    /// Computes a hash for an ION operation for Bitcoin anchoring.
    /// </summary>
    private static string ComputeOperationHash(object operation)
    {
        var json = JsonSerializer.Serialize(operation, new JsonSerializerOptions 
        { 
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}