using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Methods.MethodBase;

namespace Mamey.Identity.Decentralized.Methods.Ion;

/// <summary>
/// Implements the did:ion method (https://identity.foundation/ion/)
/// : resolves, creates, updates, and deactivates ION DIDs.
/// </summary>
public class DidIonMethod : DidMethodBase
{
    public override string Name => "ion";
    private readonly HttpClient _httpClient;
    private readonly string _ionNodeApiBaseUrl; // e.g., "https://ion.tbddev.org"

    /// <summary>
    /// Constructs an ION method handler using the specified ION node or API base URL.
    /// </summary>
    public DidIonMethod(HttpClient httpClient, string ionNodeApiBaseUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ionNodeApiBaseUrl = ionNodeApiBaseUrl?.TrimEnd('/') ??
                             throw new ArgumentNullException(nameof(ionNodeApiBaseUrl));
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

        // Poll for DID resolution or return as-is
        return await ResolveAsync(opResponse.Did, cancellationToken);
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

        var updateOp = IonOperationBuilder.BuildUpdateOperation(did, updateOptions);
        var json = JsonSerializer.Serialize(updateOp, new JsonSerializerOptions { WriteIndented = true });

        var url = $"{_ionNodeApiBaseUrl}/operations";
        var response = await _httpClient.PostAsync(url, new StringContent(json), cancellationToken);
        response.EnsureSuccessStatusCode();

        // Poll for new state or re-resolve
        return await ResolveAsync(did, cancellationToken);
    }

    /// <summary>
    /// Deactivates a did:ion by submitting a deactivate operation.
    /// </summary>
    public override async Task DeactivateAsync(string did, CancellationToken cancellationToken = default)
    {
        ValidateDid(did);

        var deactivateOp = IonOperationBuilder.BuildDeactivateOperation(did);
        var json = JsonSerializer.Serialize(deactivateOp, new JsonSerializerOptions { WriteIndented = true });

        var url = $"{_ionNodeApiBaseUrl}/operations";
        var response = await _httpClient.PostAsync(url, new StringContent(json), cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}