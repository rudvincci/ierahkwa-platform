using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mamey.FWID.Identities.Application.Clients;
using Mamey.Microservice.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Ledger;

/// <summary>
/// Service for logging identity events to FutureWampumLedger for immutable audit trail.
/// Implements privacy-preserving logging by hashing sensitive data.
/// </summary>
internal class LedgerAuditService : ILedgerAuditService
{
    private readonly ILedgerTransactionClient _ledgerClient;
    private readonly IContext _context;
    private readonly ILogger<LedgerAuditService> _logger;

    public LedgerAuditService(
        ILedgerTransactionClient ledgerClient,
        IContext context,
        ILogger<LedgerAuditService> logger)
    {
        _ledgerClient = ledgerClient;
        _context = context;
        _logger = logger;
    }

    public async Task<bool> LogIdentityCreatedAsync(
        Guid identityId,
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        var metadata = new Dictionary<string, object>
        {
            { "firstNameHash", HashSensitiveData(firstName) },
            { "lastNameHash", HashSensitiveData(lastName) },
            { "emailHash", HashSensitiveData(email) }
        };

        var request = new TransactionLogRequest
        {
            TransactionType = "IdentityCreated",
            EntityType = "Identity",
            EntityId = identityId,
            Description = $"Identity created: {identityId}",
            Metadata = metadata,
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId ?? _context.CorrelationId.ToString()
        };

        return await _ledgerClient.LogTransactionAsync(request, cancellationToken);
    }

    public async Task<bool> LogIdentityUpdatedAsync(
        Guid identityId,
        Dictionary<string, object>? changedAttributes = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        var metadata = new Dictionary<string, object>();

        if (changedAttributes != null)
        {
            foreach (var attr in changedAttributes)
            {
                // Hash sensitive attributes
                var value = attr.Value?.ToString();
                if (IsSensitiveAttribute(attr.Key))
                {
                    metadata[$"{attr.Key}Hash"] = HashSensitiveData(value);
                }
                else
                {
                    metadata[attr.Key] = attr.Value ?? "";
                }
            }
        }

        var request = new TransactionLogRequest
        {
            TransactionType = "IdentityUpdated",
            EntityType = "Identity",
            EntityId = identityId,
            Description = $"Identity updated: {identityId}",
            Metadata = metadata,
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId ?? _context.CorrelationId.ToString()
        };

        return await _ledgerClient.LogTransactionAsync(request, cancellationToken);
    }

    public async Task<bool> LogIdentityDeletedAsync(
        Guid identityId,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new TransactionLogRequest
        {
            TransactionType = "IdentityDeleted",
            EntityType = "Identity",
            EntityId = identityId,
            Description = $"Identity deleted: {identityId}",
            Metadata = new Dictionary<string, object>
            {
                { "deletedAt", DateTime.UtcNow }
            },
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId ?? _context.CorrelationId.ToString()
        };

        return await _ledgerClient.LogTransactionAsync(request, cancellationToken);
    }

    public async Task<bool> LogIdentityEventAsync(
        string eventType,
        Guid identityId,
        string? description = null,
        Dictionary<string, object>? metadata = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        // Hash sensitive data in metadata
        var safeMetadata = new Dictionary<string, object>();
        if (metadata != null)
        {
            foreach (var item in metadata)
            {
                if (IsSensitiveAttribute(item.Key))
                {
                    safeMetadata[$"{item.Key}Hash"] = HashSensitiveData(item.Value?.ToString());
                }
                else
                {
                    safeMetadata[item.Key] = item.Value ?? "";
                }
            }
        }

        var request = new TransactionLogRequest
        {
            TransactionType = eventType,
            EntityType = "Identity",
            EntityId = identityId,
            Description = description ?? $"Identity event: {eventType}",
            Metadata = safeMetadata,
            Timestamp = DateTime.UtcNow,
            CorrelationId = correlationId ?? _context.CorrelationId.ToString()
        };

        return await _ledgerClient.LogTransactionAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyList<AuditTrailEntry>> QueryAuditTrailAsync(
        Guid identityId,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? eventType = null,
        CancellationToken cancellationToken = default)
    {
        // In a real implementation, this would query the ledger service
        // For now, return empty list as the ledger service query API may not be implemented yet
        _logger.LogInformation(
            "Querying audit trail for IdentityId: {IdentityId}, From: {FromDate}, To: {ToDate}, EventType: {EventType}",
            identityId,
            fromDate,
            toDate,
            eventType);

        // TODO: Implement actual query when ledger service provides query API
        return Array.Empty<AuditTrailEntry>();
    }

    private static string HashSensitiveData(string? data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return "";
        }

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hashBytes);
    }

    private static bool IsSensitiveAttribute(string attributeName)
    {
        var sensitiveAttributes = new[]
        {
            "firstName", "lastName", "email", "phone", "address",
            "ssn", "passport", "dateOfBirth", "biometric"
        };

        return sensitiveAttributes.Any(attr =>
            attributeName.Contains(attr, StringComparison.OrdinalIgnoreCase));
    }
}
