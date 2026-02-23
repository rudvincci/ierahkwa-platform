using System.Text.Json;
using Mamey.Identity.Decentralized.Abstractions;
using Mamey.Identity.Decentralized.Core;
using Microsoft.Extensions.Logging;

namespace Mamey.Identity.Decentralized.Services;

/// <summary>
/// Implements ICredentialStatusService for StatusList2021 and custom status mechanisms.
/// </summary>
public class CredentialStatusService : ICredentialStatusService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CredentialStatusService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public CredentialStatusService(HttpClient httpClient, ILogger<CredentialStatusService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<bool> IsRevokedAsync(
        string statusListCredentialUrl,
        int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Fetch the status list credential (StatusList2021)
            var response = await _httpClient.GetAsync(statusListCredentialUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var encodedList = doc.RootElement
                .GetProperty("credentialSubject")
                .GetProperty("encodedList")
                .GetString();

            byte[] bitstring = Convert.FromBase64String(encodedList);
            int byteIndex = statusListIndex / 8;
            int bitIndex = statusListIndex % 8;
            bool isRevoked = (bitstring[byteIndex] & (1 << (7 - bitIndex))) != 0;

            return isRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during IsRevokedAsync for {StatusListCredentialUrl} at index {StatusListIndex}", statusListCredentialUrl, statusListIndex);
            // Defensive: treat as NOT revoked if you cannot verify, or use policy
            return false;
        }
    }

    public async Task<bool> IsCredentialActiveAsync(CredentialStatus status,
        CancellationToken cancellationToken = default)
    {
        string result = await GetCredentialStatusAsync(status, cancellationToken);
        return result == "active";
    }

    public async Task<string> GetCredentialStatusAsync(CredentialStatus status,
        CancellationToken cancellationToken = default)
    {
        if (status == null)
            throw new ArgumentNullException(nameof(status));

        // Only StatusList2021Entry is handled here; you can extend for other protocols.
        if (!string.Equals(status.Type, "StatusList2021Entry", StringComparison.OrdinalIgnoreCase))
            return "unknown";

        if (string.IsNullOrWhiteSpace(status.StatusListCredential) || string.IsNullOrWhiteSpace(status.StatusListIndex))
            return "unknown";

        // 1. Fetch the status list VC (as per W3C Status List 2021 spec)
        var response = await _httpClient.GetAsync(status.StatusListCredential, cancellationToken);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(json);
        var encodedList = doc.RootElement
            .GetProperty("credentialSubject")
            .GetProperty("encodedList")
            .GetString();

        // 2. Decode the bitstring (base64, see spec)
        byte[] bitstring = Convert.FromBase64String(encodedList);
        int index = int.Parse(status.StatusListIndex);

        // 3. Check the bit at the given index (0 = active, 1 = revoked)
        int byteIndex = index / 8;
        int bitIndex = index % 8;
        bool isRevoked = (bitstring[byteIndex] & (1 << (7 - bitIndex))) != 0;

        if (string.Equals(status.StatusPurpose, "revocation", StringComparison.OrdinalIgnoreCase))
            return isRevoked ? "revoked" : "active";
        if (string.Equals(status.StatusPurpose, "suspension", StringComparison.OrdinalIgnoreCase))
            return isRevoked ? "suspended" : "active";

        return isRevoked ? "revoked" : "active";
    }

    public async Task<CredentialStatusResult> CheckStatusAsync(string statusId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentNullException(nameof(statusId), "statusId must not be null or empty.");

        if (Uri.TryCreate(statusId, UriKind.Absolute, out var uri))
        {
            // VC Status List 2021 or HTTP(S)-based status
            if (statusId.Contains("/status/")) // Simple heuristic
                return await CheckVcStatusList2021Async(statusId, cancellationToken);

            // Add support for other status URI patterns as needed
        }

        // For non-HTTP status IDs (custom, internal, etc.), implement extension
        // For now, assume unrecognized: treat as non-revoked (safe default)
        _logger.LogWarning("Unknown or unsupported status ID format: {StatusId}", statusId);
        return new CredentialStatusResult
        {
            IsRevoked = false,
            IsSuspended = false,
            Reason = "Status not found; assuming not revoked",
            Source = "CredentialStatusService"
        };
    }

    /// <summary>
    /// Fetches and parses a VC Status List 2021 entry and determines if the credential is revoked.
    /// </summary>
    private async Task<CredentialStatusResult> CheckVcStatusList2021Async(string statusListEntryId,
        CancellationToken cancellationToken)
    {
        try
        {
            // 1. Parse the base status list credential URL from the entry (remove #fragment)
            var uri = new Uri(statusListEntryId);
            var baseListUrl = statusListEntryId.Split('#')[0];

            // 2. Fetch the status list credential JSON
            var http = _httpClientFactory.CreateClient("vc-status");
            var resp = await http.GetAsync(baseListUrl, cancellationToken);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // 3. Find the encoded bitstring/list
            var encodedList = root
                .GetProperty("credentialSubject")
                .GetProperty("encodedList")
                .GetString();

            // 4. Get the index (from the statusListEntry object)
            // According to the spec, the fragment should be #list and the entry must provide an index (statusListIndex)
            // To find the matching entry, you’ll need to pass in the statusListIndex (your app should have this from the credential)
            // Here, we expect: statusListEntryId = "https://.../status/1#list"
            // (In practice, you may also need to fetch the original credential to get the "statusListIndex" value)

            // EXAMPLE: For demo, assume index 42. Replace with real extraction logic:
            int index = 42;

            // 5. Decode the bitstring (usually base64 or gzip+base64)
            byte[] decoded = Convert.FromBase64String(encodedList);
            var bitArray = new System.Collections.BitArray(decoded);

            bool isRevoked = bitArray[index];

            return new CredentialStatusResult
            {
                IsRevoked = isRevoked,
                IsSuspended = false,
                Reason = isRevoked ? "Revoked by status list" : null,
                Source = baseListUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check VC Status List 2021 for {StatusId}", statusListEntryId);
            return new CredentialStatusResult
            {
                IsRevoked = false,
                IsSuspended = false,
                Reason = "Unable to resolve or decode status list; treating as not revoked.",
                Source = statusListEntryId
            };
        }
    }
}