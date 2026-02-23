using System.Net.NetworkInformation;
using Mamey.FWID.Identities.Application.Offline.Models;
using Mamey.FWID.Identities.Application.Offline.Storage;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Offline.Services;

/// <summary>
/// Interface for synchronization service.
/// Handles sync between offline cache and server when connectivity is restored.
/// </summary>
public interface ISyncService
{
    /// <summary>
    /// Starts the synchronization process.
    /// </summary>
    Task<SyncResult> SyncAsync(SyncOptions? options = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if connectivity is available.
    /// </summary>
    Task<bool> IsConnectedAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the sync status.
    /// </summary>
    SyncStatus GetStatus();
    
    /// <summary>
    /// Queues a credential for sync.
    /// </summary>
    Task QueueCredentialSyncAsync(CachedCredential credential, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Queues audit log entries for sync.
    /// </summary>
    Task QueueAuditLogSyncAsync(IEnumerable<AuditLogEntry> entries, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Event raised when sync completes.
    /// </summary>
    event EventHandler<SyncCompletedEventArgs>? SyncCompleted;
}

/// <summary>
/// Sync options.
/// </summary>
public class SyncOptions
{
    /// <summary>
    /// Whether to sync audit logs.
    /// </summary>
    public bool SyncAuditLogs { get; set; } = true;
    
    /// <summary>
    /// Whether to sync credentials.
    /// </summary>
    public bool SyncCredentials { get; set; } = true;
    
    /// <summary>
    /// Whether to sync biometric verification results.
    /// </summary>
    public bool SyncVerificationResults { get; set; } = true;
    
    /// <summary>
    /// Whether to update cached templates.
    /// </summary>
    public bool UpdateTemplates { get; set; } = true;
    
    /// <summary>
    /// Timeout for the sync operation.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    /// <summary>
    /// Whether to force sync even if recent sync occurred.
    /// </summary>
    public bool Force { get; set; } = false;
}

/// <summary>
/// Result of a sync operation.
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public int AuditLogsSynced { get; set; }
    public int CredentialsSynced { get; set; }
    public int VerificationResultsSynced { get; set; }
    public int TemplatesUpdated { get; set; }
    public int ConflictsResolved { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan Duration { get; set; }
    
    public static SyncResult SuccessResult(int auditLogs, int credentials, int verifications, int templates)
    {
        return new SyncResult
        {
            Success = true,
            AuditLogsSynced = auditLogs,
            CredentialsSynced = credentials,
            VerificationResultsSynced = verifications,
            TemplatesUpdated = templates
        };
    }
    
    public static SyncResult FailedResult(string error)
    {
        return new SyncResult
        {
            Success = false,
            Errors = new List<string> { error }
        };
    }
}

/// <summary>
/// Current sync status.
/// </summary>
public class SyncStatus
{
    public bool IsSyncing { get; set; }
    public DateTime? LastSyncAt { get; set; }
    public DateTime? LastSuccessfulSyncAt { get; set; }
    public int PendingAuditLogs { get; set; }
    public int PendingCredentials { get; set; }
    public int PendingVerificationResults { get; set; }
    public bool IsConnected { get; set; }
}

/// <summary>
/// Event args for sync completion.
/// </summary>
public class SyncCompletedEventArgs : EventArgs
{
    public SyncResult Result { get; }
    public SyncCompletedEventArgs(SyncResult result) => Result = result;
}

/// <summary>
/// Audit log entry for offline operations.
/// </summary>
public class AuditLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string EventType { get; set; } = null!;
    public Guid? IdentityId { get; set; }
    public string? DeviceId { get; set; }
    public Dictionary<string, object>? Data { get; set; }
    public bool IsSynced { get; set; }
}

/// <summary>
/// Synchronization service implementation.
/// </summary>
public class SyncService : ISyncService
{
    private readonly ISecureLocalStorage _storage;
    private readonly IOfflineBiometricService _biometricService;
    private readonly ILogger<SyncService> _logger;
    
    private readonly List<AuditLogEntry> _pendingAuditLogs = new();
    private readonly List<CachedCredential> _pendingCredentials = new();
    private readonly object _syncLock = new();
    
    private SyncStatus _status = new();
    
    public event EventHandler<SyncCompletedEventArgs>? SyncCompleted;
    
    public SyncService(
        ISecureLocalStorage storage,
        IOfflineBiometricService biometricService,
        ILogger<SyncService> logger)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _biometricService = biometricService ?? throw new ArgumentNullException(nameof(biometricService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    /// <inheritdoc />
    public async Task<SyncResult> SyncAsync(SyncOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new SyncOptions();
        var startTime = DateTime.UtcNow;
        
        _logger.LogInformation("Starting sync operation");
        
        lock (_syncLock)
        {
            if (_status.IsSyncing && !options.Force)
            {
                _logger.LogWarning("Sync already in progress");
                return SyncResult.FailedResult("Sync already in progress");
            }
            _status.IsSyncing = true;
            _status.LastSyncAt = startTime;
        }
        
        try
        {
            // Check connectivity
            var isConnected = await IsConnectedAsync(cancellationToken);
            if (!isConnected)
            {
                _logger.LogWarning("No connectivity available for sync");
                return SyncResult.FailedResult("No network connectivity");
            }
            
            var auditLogsSynced = 0;
            var credentialsSynced = 0;
            var verificationsSynced = 0;
            var templatesUpdated = 0;
            var errors = new List<string>();
            
            // Sync audit logs first (priority)
            if (options.SyncAuditLogs)
            {
                try
                {
                    auditLogsSynced = await SyncAuditLogsAsync(cancellationToken);
                    _logger.LogInformation("Synced {Count} audit logs", auditLogsSynced);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing audit logs");
                    errors.Add($"Audit log sync failed: {ex.Message}");
                }
            }
            
            // Sync verification results
            if (options.SyncVerificationResults)
            {
                try
                {
                    verificationsSynced = await SyncVerificationResultsAsync(cancellationToken);
                    _logger.LogInformation("Synced {Count} verification results", verificationsSynced);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing verification results");
                    errors.Add($"Verification sync failed: {ex.Message}");
                }
            }
            
            // Sync credentials
            if (options.SyncCredentials)
            {
                try
                {
                    credentialsSynced = await SyncCredentialsAsync(cancellationToken);
                    _logger.LogInformation("Synced {Count} credentials", credentialsSynced);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing credentials");
                    errors.Add($"Credential sync failed: {ex.Message}");
                }
            }
            
            // Update templates from server
            if (options.UpdateTemplates)
            {
                try
                {
                    templatesUpdated = await UpdateTemplatesFromServerAsync(cancellationToken);
                    _logger.LogInformation("Updated {Count} templates", templatesUpdated);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating templates");
                    errors.Add($"Template update failed: {ex.Message}");
                }
            }
            
            var result = new SyncResult
            {
                Success = errors.Count == 0,
                AuditLogsSynced = auditLogsSynced,
                CredentialsSynced = credentialsSynced,
                VerificationResultsSynced = verificationsSynced,
                TemplatesUpdated = templatesUpdated,
                Errors = errors,
                Duration = DateTime.UtcNow - startTime
            };
            
            lock (_syncLock)
            {
                _status.LastSuccessfulSyncAt = result.Success ? DateTime.UtcNow : _status.LastSuccessfulSyncAt;
            }
            
            SyncCompleted?.Invoke(this, new SyncCompletedEventArgs(result));
            
            _logger.LogInformation("Sync completed. Success: {Success}, Duration: {Duration}ms",
                result.Success, result.Duration.TotalMilliseconds);
                
            return result;
        }
        finally
        {
            lock (_syncLock)
            {
                _status.IsSyncing = false;
            }
        }
    }
    
    /// <inheritdoc />
    public Task<bool> IsConnectedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Simple connectivity check
            var connected = NetworkInterface.GetIsNetworkAvailable();
            
            lock (_syncLock)
            {
                _status.IsConnected = connected;
            }
            
            return Task.FromResult(connected);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error checking connectivity");
            return Task.FromResult(false);
        }
    }
    
    /// <inheritdoc />
    public SyncStatus GetStatus()
    {
        lock (_syncLock)
        {
            return new SyncStatus
            {
                IsSyncing = _status.IsSyncing,
                LastSyncAt = _status.LastSyncAt,
                LastSuccessfulSyncAt = _status.LastSuccessfulSyncAt,
                PendingAuditLogs = _pendingAuditLogs.Count(l => !l.IsSynced),
                PendingCredentials = _pendingCredentials.Count,
                PendingVerificationResults = 0, // Get from biometric service
                IsConnected = _status.IsConnected
            };
        }
    }
    
    /// <inheritdoc />
    public Task QueueCredentialSyncAsync(CachedCredential credential, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            _pendingCredentials.Add(credential);
        }
        _logger.LogDebug("Queued credential for sync: {CredentialId}", credential.Id);
        return Task.CompletedTask;
    }
    
    /// <inheritdoc />
    public Task QueueAuditLogSyncAsync(IEnumerable<AuditLogEntry> entries, CancellationToken cancellationToken = default)
    {
        lock (_syncLock)
        {
            _pendingAuditLogs.AddRange(entries);
        }
        _logger.LogDebug("Queued {Count} audit log entries for sync", entries.Count());
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Syncs audit logs to the server.
    /// </summary>
    private async Task<int> SyncAuditLogsAsync(CancellationToken cancellationToken)
    {
        List<AuditLogEntry> toSync;
        lock (_syncLock)
        {
            toSync = _pendingAuditLogs.Where(l => !l.IsSynced).ToList();
        }
        
        if (!toSync.Any())
            return 0;
            
        // In production, send to server API
        // POST /api/audit/offline
        await Task.Delay(100, cancellationToken); // Simulate network
        
        // Mark as synced
        foreach (var entry in toSync)
        {
            entry.IsSynced = true;
        }
        
        lock (_syncLock)
        {
            _pendingAuditLogs.RemoveAll(l => l.IsSynced);
        }
        
        return toSync.Count;
    }
    
    /// <summary>
    /// Syncs verification results to the server.
    /// </summary>
    private async Task<int> SyncVerificationResultsAsync(CancellationToken cancellationToken)
    {
        var pending = await _biometricService.GetPendingSyncResultsAsync(cancellationToken);
        if (!pending.Any())
            return 0;
            
        // In production, send to server API
        // POST /api/verifications/offline
        await Task.Delay(100, cancellationToken); // Simulate network
        
        // Mark as synced
        await _biometricService.MarkSyncedAsync(pending.Select(r => r.TransactionId), cancellationToken);
        
        return pending.Count;
    }
    
    /// <summary>
    /// Syncs credentials updates to the server.
    /// </summary>
    private async Task<int> SyncCredentialsAsync(CancellationToken cancellationToken)
    {
        List<CachedCredential> toSync;
        lock (_syncLock)
        {
            toSync = _pendingCredentials.ToList();
        }
        
        if (!toSync.Any())
            return 0;
            
        // In production, send updates and fetch revocation status
        await Task.Delay(100, cancellationToken); // Simulate network
        
        lock (_syncLock)
        {
            _pendingCredentials.Clear();
        }
        
        return toSync.Count;
    }
    
    /// <summary>
    /// Updates templates from the server.
    /// </summary>
    private async Task<int> UpdateTemplatesFromServerAsync(CancellationToken cancellationToken)
    {
        // In production, fetch updated templates from server
        // GET /api/templates/updates?since={lastSync}
        await Task.Delay(100, cancellationToken); // Simulate network
        
        // Cleanup expired entries
        await _storage.CleanupExpiredEntriesAsync(cancellationToken);
        
        return 0; // Number of templates updated
    }
}
