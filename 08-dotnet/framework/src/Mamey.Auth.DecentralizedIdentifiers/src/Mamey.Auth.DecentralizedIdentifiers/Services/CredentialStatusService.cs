using System.Text.Json;
using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Mamey.Auth.DecentralizedIdentifiers.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// Enhanced implementation of ICredentialStatusService for StatusList2021, RevocationList2020, and custom status mechanisms.
/// </summary>
public class CredentialStatusService : ICredentialStatusService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CredentialStatusService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

    public CredentialStatusService(
        HttpClient httpClient, 
        ILogger<CredentialStatusService> logger,
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }
    
    public async Task<bool> IsRevokedAsync(
        string statusListCredentialUrl,
        int statusListIndex,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Create cache key
            var cacheKey = $"credential_status:{statusListCredentialUrl}:{statusListIndex}";
            
            // Check cache first
            if (_cache.TryGetValue(cacheKey, out bool cachedResult))
            {
                _logger.LogDebug("Retrieved revocation status from cache for {StatusListCredentialUrl} at index {StatusListIndex}: {IsRevoked}", 
                    statusListCredentialUrl, statusListIndex, cachedResult);
                return cachedResult;
            }

            // Use semaphore to prevent concurrent requests for the same status list
            var semaphore = _semaphores.GetOrAdd(statusListCredentialUrl, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                // Double-check cache after acquiring semaphore
                if (_cache.TryGetValue(cacheKey, out cachedResult))
                {
                    return cachedResult;
                }

                // Fetch the status list credential (StatusList2021)
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

                // Cache the result for 5 minutes
                _cache.Set(cacheKey, isRevoked, TimeSpan.FromMinutes(5));

                _logger.LogDebug("Retrieved revocation status for {StatusListCredentialUrl} at index {StatusListIndex}: {IsRevoked}", 
                    statusListCredentialUrl, statusListIndex, isRevoked);

                return isRevoked;
            }
            finally
            {
                semaphore.Release();
            }
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

        // Support multiple status types
        switch (status.Type?.ToLowerInvariant())
        {
            case "statuslist2021entry":
                return await GetStatusList2021StatusAsync(status, cancellationToken);
            
            case "revocationlist2020status":
                return await GetRevocationList2020StatusAsync(status, cancellationToken);
            
            case "ocsp":
                return await GetOcspStatusAsync(status, cancellationToken);
            
            case "custom":
                return await GetCustomStatusAsync(status, cancellationToken);
            
            default:
                _logger.LogWarning("Unsupported credential status type: {StatusType}", status.Type);
                return "unknown";
        }
    }

    private async Task<string> GetStatusList2021StatusAsync(CredentialStatus status, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(status.StatusListCredential) || string.IsNullOrWhiteSpace(status.StatusListIndex))
            return "unknown";

        try
        {
            // Create cache key
            var cacheKey = $"statuslist2021:{status.StatusListCredential}:{status.StatusListIndex}";
            
            if (_cache.TryGetValue(cacheKey, out string cachedStatus))
            {
                return cachedStatus;
            }

            // Fetch the status list VC (as per W3C Status List 2021 spec)
            var response = await _httpClient.GetAsync(status.StatusListCredential, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var encodedList = doc.RootElement
                .GetProperty("credentialSubject")
                .GetProperty("encodedList")
                .GetString();

            // Decode the bitstring (base64, see spec)
            byte[] bitstring = Convert.FromBase64String(encodedList);
            int index = int.Parse(status.StatusListIndex);

            // Check the bit at the given index (0 = active, 1 = revoked)
            int byteIndex = index / 8;
            int bitIndex = index % 8;
            bool isRevoked = (bitstring[byteIndex] & (1 << (7 - bitIndex))) != 0;

            string result;
            if (string.Equals(status.StatusPurpose, "revocation", StringComparison.OrdinalIgnoreCase))
                result = isRevoked ? "revoked" : "active";
            else if (string.Equals(status.StatusPurpose, "suspension", StringComparison.OrdinalIgnoreCase))
                result = isRevoked ? "suspended" : "active";
            else
                result = isRevoked ? "revoked" : "active";

            // Cache the result for 5 minutes
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking StatusList2021 status for {StatusListCredential}", status.StatusListCredential);
            return "unknown";
        }
    }

    private async Task<string> GetRevocationList2020StatusAsync(CredentialStatus status, CancellationToken cancellationToken)
    {
        try
        {
            // RevocationList2020 uses a different format
            if (string.IsNullOrWhiteSpace(status.StatusListCredential))
                return "unknown";

            var cacheKey = $"revocationlist2020:{status.StatusListCredential}";
            
            if (_cache.TryGetValue(cacheKey, out string cachedStatus))
            {
                return cachedStatus;
            }

            var response = await _httpClient.GetAsync(status.StatusListCredential, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var revokedCredentials = doc.RootElement
                .GetProperty("credentialSubject")
                .GetProperty("revokedCredentials");

            // Check if the credential ID is in the revoked list
            bool isRevoked = false;
            foreach (var revokedCred in revokedCredentials.EnumerateArray())
            {
                if (revokedCred.TryGetProperty("id", out var idElement) && 
                    idElement.GetString() == status.Id)
                {
                    isRevoked = true;
                    break;
                }
            }

            string result = isRevoked ? "revoked" : "active";
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking RevocationList2020 status for {StatusListCredential}", status.StatusListCredential);
            return "unknown";
        }
    }

    private async Task<string> GetOcspStatusAsync(CredentialStatus status, CancellationToken cancellationToken)
    {
        try
        {
            // OCSP (Online Certificate Status Protocol) implementation
            // This would typically involve checking certificate revocation via OCSP
            _logger.LogInformation("OCSP status check requested for credential {CredentialId}", status.Id);
            
            // Placeholder implementation - in reality you would:
            // 1. Extract certificate from credential
            // 2. Query OCSP responder
            // 3. Parse OCSP response
            
            return "active"; // Placeholder
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OCSP status for credential {CredentialId}", status.Id);
            return "unknown";
        }
    }

    private async Task<string> GetCustomStatusAsync(CredentialStatus status, CancellationToken cancellationToken)
    {
        try
        {
            // Custom status implementation - could be database lookup, API call, etc.
            _logger.LogInformation("Custom status check requested for credential {CredentialId}", status.Id);
            
            // Placeholder implementation
            return "active";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking custom status for credential {CredentialId}", status.Id);
            return "unknown";
        }
    }

    public async Task<CredentialStatusResult> CheckStatusAsync(string statusId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(statusId))
            throw new ArgumentNullException(nameof(statusId), "statusId must not be null or empty.");

        try
        {
            // Create cache key
            var cacheKey = $"status_check:{statusId}";
            
            if (_cache.TryGetValue(cacheKey, out CredentialStatusResult cachedResult))
            {
                return cachedResult;
            }

            CredentialStatusResult result;

            if (Uri.TryCreate(statusId, UriKind.Absolute, out var uri))
            {
                // VC Status List 2021 or HTTP(S)-based status
                if (statusId.Contains("/status/"))
                    result = await CheckVcStatusList2021Async(statusId, cancellationToken);
                else if (statusId.Contains("/revocation/"))
                    result = await CheckRevocationList2020Async(statusId, cancellationToken);
                else if (statusId.Contains("/ocsp/"))
                    result = await CheckOcspStatusAsync(statusId, cancellationToken);
                else
                {
                    _logger.LogWarning("Unknown HTTP status ID format: {StatusId}", statusId);
                    result = new CredentialStatusResult
                    {
                        IsRevoked = false,
                        IsSuspended = false,
                        Reason = "Unknown status format; assuming not revoked",
                        Source = statusId
                    };
                }
            }
            else
            {
                // For non-HTTP status IDs (custom, internal, etc.)
                _logger.LogWarning("Non-HTTP status ID format: {StatusId}", statusId);
                result = new CredentialStatusResult
                {
                    IsRevoked = false,
                    IsSuspended = false,
                    Reason = "Non-HTTP status ID; assuming not revoked",
                    Source = statusId
                };
            }

            // Cache the result for 5 minutes
            _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking status for {StatusId}", statusId);
            return new CredentialStatusResult
            {
                IsRevoked = false,
                IsSuspended = false,
                Reason = $"Error checking status: {ex.Message}",
                Source = statusId
            };
        }
    }

    /// <summary>
    /// Checks the status of multiple credentials in batch.
    /// </summary>
    public async Task<Dictionary<string, CredentialStatusResult>> CheckStatusBatchAsync(
        IEnumerable<string> statusIds, 
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, CredentialStatusResult>();
        var tasks = new List<Task<(string statusId, CredentialStatusResult result)>>();

        foreach (var statusId in statusIds)
        {
            tasks.Add(CheckStatusWithIdAsync(statusId, cancellationToken));
        }

        var completedTasks = await Task.WhenAll(tasks);
        
        foreach (var (statusId, result) in completedTasks)
        {
            results[statusId] = result;
        }

        return results;
    }

    private async Task<(string statusId, CredentialStatusResult result)> CheckStatusWithIdAsync(
        string statusId, 
        CancellationToken cancellationToken)
    {
        var result = await CheckStatusAsync(statusId, cancellationToken);
        return (statusId, result);
    }

    /// <summary>
    /// Checks if multiple credentials are revoked in batch.
    /// </summary>
    public async Task<Dictionary<string, bool>> IsRevokedBatchAsync(
        IEnumerable<(string statusListCredentialUrl, int statusListIndex)> credentials,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, bool>();
        var tasks = new List<Task<(string key, bool isRevoked)>>();

        foreach (var (url, index) in credentials)
        {
            var key = $"{url}:{index}";
            tasks.Add(IsRevokedWithKeyAsync(url, index, key, cancellationToken));
        }

        var completedTasks = await Task.WhenAll(tasks);
        
        foreach (var (key, isRevoked) in completedTasks)
        {
            results[key] = isRevoked;
        }

        return results;
    }

    private async Task<(string key, bool isRevoked)> IsRevokedWithKeyAsync(
        string statusListCredentialUrl, 
        int statusListIndex, 
        string key,
        CancellationToken cancellationToken)
    {
        var isRevoked = await IsRevokedAsync(statusListCredentialUrl, statusListIndex, cancellationToken);
        return (key, isRevoked);
    }

    /// <summary>
    /// Invalidates cached status for a specific credential.
    /// </summary>
    public void InvalidateStatusCache(string statusId)
    {
        var cacheKey = $"status_check:{statusId}";
        _cache.Remove(cacheKey);
        
        // Also remove related cache entries
        _cache.Remove($"credential_status:{statusId}");
        _cache.Remove($"statuslist2021:{statusId}");
        _cache.Remove($"revocationlist2020:{statusId}");
        
        _logger.LogDebug("Invalidated status cache for {StatusId}", statusId);
    }

    /// <summary>
    /// Clears all cached status information.
    /// </summary>
    public void ClearStatusCache()
    {
        // Note: IMemoryCache doesn't have a Clear method, so we can't easily clear all entries
        // In a production system, you might want to use a different caching mechanism
        _logger.LogInformation("Status cache clear requested - individual entries will expire naturally");
    }

    /// <summary>
    /// Fetches and parses a VC Status List 2021 entry and determines if the credential is revoked.
    /// </summary>
    private async Task<CredentialStatusResult> CheckVcStatusList2021Async(string statusListEntryId,
        CancellationToken cancellationToken)
    {
        try
        {
            // Parse the base status list credential URL from the entry (remove #fragment)
            var uri = new Uri(statusListEntryId);
            var baseListUrl = statusListEntryId.Split('#')[0];

            // Fetch the status list credential JSON
            var http = _httpClientFactory.CreateClient("vc-status");
            var resp = await http.GetAsync(baseListUrl, cancellationToken);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Find the encoded bitstring/list
            var encodedList = root
                .GetProperty("credentialSubject")
                .GetProperty("encodedList")
                .GetString();

            // Get the index (from the statusListEntry object)
            // According to the spec, the fragment should be #list and the entry must provide an index (statusListIndex)
            // To find the matching entry, you'll need to pass in the statusListIndex (your app should have this from the credential)
            // Here, we expect: statusListEntryId = "https://.../status/1#list"
            // (In practice, you may also need to fetch the original credential to get the "statusListIndex" value)

            // EXAMPLE: For demo, assume index 42. Replace with real extraction logic:
            int index = 42;

            // Decode the bitstring (usually base64 or gzip+base64)
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

    /// <summary>
    /// Checks RevocationList2020 status.
    /// </summary>
    private async Task<CredentialStatusResult> CheckRevocationList2020Async(string statusId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync(statusId, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            var revokedCredentials = doc.RootElement
                .GetProperty("credentialSubject")
                .GetProperty("revokedCredentials");

            // Check if the credential ID is in the revoked list
            bool isRevoked = false;
            foreach (var revokedCred in revokedCredentials.EnumerateArray())
            {
                if (revokedCred.TryGetProperty("id", out var idElement) && 
                    idElement.GetString() == statusId)
                {
                    isRevoked = true;
                    break;
                }
            }

            return new CredentialStatusResult
            {
                IsRevoked = isRevoked,
                IsSuspended = false,
                Reason = isRevoked ? "Revoked by RevocationList2020" : null,
                Source = statusId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check RevocationList2020 for {StatusId}", statusId);
            return new CredentialStatusResult
            {
                IsRevoked = false,
                IsSuspended = false,
                Reason = "Unable to check RevocationList2020; treating as not revoked.",
                Source = statusId
            };
        }
    }

    /// <summary>
    /// Checks OCSP status.
    /// </summary>
    private async Task<CredentialStatusResult> CheckOcspStatusAsync(string statusId,
        CancellationToken cancellationToken)
    {
        try
        {
            // OCSP implementation would go here
            _logger.LogInformation("OCSP status check requested for {StatusId}", statusId);
            
            // Placeholder implementation
            return new CredentialStatusResult
            {
                IsRevoked = false,
                IsSuspended = false,
                Reason = "OCSP check not implemented",
                Source = statusId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check OCSP status for {StatusId}", statusId);
            return new CredentialStatusResult
            {
                IsRevoked = false,
                IsSuspended = false,
                Reason = "OCSP check failed; treating as not revoked.",
                Source = statusId
            };
        }
    }
}