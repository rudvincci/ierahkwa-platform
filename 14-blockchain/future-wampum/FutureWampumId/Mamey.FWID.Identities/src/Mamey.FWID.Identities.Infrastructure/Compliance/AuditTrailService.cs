using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Application.Services;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Infrastructure.Compliance;

/// <summary>
/// Service implementation for compliance audit trail operations.
/// Integrates with MameyNode ComplianceService for AML/KYC regulatory compliance.
/// 
/// TDD Reference: Lines 1476-1498 (Compliance Requirements)
/// BDD Reference: Lines 645-692 (VII. Compliance and Regulatory Framework)
/// </summary>
internal sealed class AuditTrailService : IAuditTrailService
{
    private const string SourceService = "Mamey.FWID.Identities";
    private const string EntityTypeIdentity = "Identity";

    private readonly IComplianceClient _complianceClient;
    private readonly ILogger<AuditTrailService> _logger;

    public AuditTrailService(
        IComplianceClient complianceClient,
        ILogger<AuditTrailService> logger)
    {
        _complianceClient = complianceClient ?? throw new ArgumentNullException(nameof(complianceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AuditEntryResult> CreateAuditEntryAsync(
        string entityType,
        string entityId,
        string action,
        string actor,
        Dictionary<string, string>? details = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Creating audit entry: EntityType={EntityType}, EntityId={EntityId}, Action={Action}",
            entityType, entityId, action);

        try
        {
            var request = new CreateAuditEntryRequest(
                EntityType: entityType,
                EntityId: entityId,
                Action: action,
                Actor: actor,
                Details: details);

            var response = await _complianceClient.CreateAuditEntryAsync(request, cancellationToken);

            if (response.Success)
            {
                _logger.LogDebug(
                    "Successfully created audit entry: AuditEntryId={AuditEntryId}",
                    response.AuditEntryId);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to create audit entry: Error={Error}",
                    response.ErrorMessage);
            }

            return new AuditEntryResult(
                Success: response.Success,
                AuditEntryId: response.AuditEntryId,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit entry for {EntityType}:{EntityId}", entityType, entityId);
            return new AuditEntryResult(false, null, ex.Message);
        }
    }

    public async Task<AuditEntryResult> LogKycVerificationAsync(
        string identityId,
        string verificationType,
        bool verified,
        string kycLevel,
        IReadOnlyList<string> verifiedAttributes,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging KYC verification for identity: IdentityId={IdentityId}, Type={Type}, Verified={Verified}",
            identityId, verificationType, verified);

        var details = new Dictionary<string, string>
        {
            ["verificationType"] = verificationType,
            ["verified"] = verified.ToString().ToLowerInvariant(),
            ["kycLevel"] = kycLevel,
            ["verifiedAttributes"] = string.Join(",", verifiedAttributes),
            ["sourceService"] = SourceService,
            ["timestamp"] = DateTime.UtcNow.ToString("O")
        };

        if (!string.IsNullOrEmpty(correlationId))
        {
            details["correlationId"] = correlationId;
        }

        return await CreateAuditEntryAsync(
            EntityTypeIdentity,
            identityId,
            "KycVerification",
            SourceService,
            details,
            cancellationToken);
    }

    public async Task<AuditEntryResult> LogAmlCheckAsync(
        string identityId,
        string? transactionId,
        bool flagged,
        string riskLevel,
        IReadOnlyList<string>? riskFactors = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging AML check for identity: IdentityId={IdentityId}, Flagged={Flagged}, RiskLevel={RiskLevel}",
            identityId, flagged, riskLevel);

        var details = new Dictionary<string, string>
        {
            ["flagged"] = flagged.ToString().ToLowerInvariant(),
            ["riskLevel"] = riskLevel,
            ["sourceService"] = SourceService,
            ["timestamp"] = DateTime.UtcNow.ToString("O")
        };

        if (!string.IsNullOrEmpty(transactionId))
        {
            details["transactionId"] = transactionId;
        }

        if (riskFactors?.Count > 0)
        {
            details["riskFactors"] = string.Join(",", riskFactors);
        }

        if (!string.IsNullOrEmpty(correlationId))
        {
            details["correlationId"] = correlationId;
        }

        return await CreateAuditEntryAsync(
            EntityTypeIdentity,
            identityId,
            "AmlCheck",
            SourceService,
            details,
            cancellationToken);
    }

    public async Task<AuditEntryResult> LogIdentityCreatedAsync(
        string identityId,
        string? zone,
        string? clanRegistrarId,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging identity creation for compliance: IdentityId={IdentityId}, Zone={Zone}",
            identityId, zone);

        var details = new Dictionary<string, string>
        {
            ["sourceService"] = SourceService,
            ["timestamp"] = DateTime.UtcNow.ToString("O"),
            ["eventType"] = "IdentityCreated"
        };

        if (!string.IsNullOrEmpty(zone))
        {
            details["zone"] = zone;
        }

        if (!string.IsNullOrEmpty(clanRegistrarId))
        {
            details["clanRegistrarId"] = clanRegistrarId;
        }

        if (!string.IsNullOrEmpty(correlationId))
        {
            details["correlationId"] = correlationId;
        }

        return await CreateAuditEntryAsync(
            EntityTypeIdentity,
            identityId,
            "IdentityCreated",
            SourceService,
            details,
            cancellationToken);
    }

    public async Task<AuditEntryResult> LogIdentityVerifiedAsync(
        string identityId,
        string verificationType,
        bool verified,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Logging identity verification for compliance: IdentityId={IdentityId}, Type={Type}, Verified={Verified}",
            identityId, verificationType, verified);

        var details = new Dictionary<string, string>
        {
            ["verificationType"] = verificationType,
            ["verified"] = verified.ToString().ToLowerInvariant(),
            ["sourceService"] = SourceService,
            ["timestamp"] = DateTime.UtcNow.ToString("O"),
            ["eventType"] = "IdentityVerified"
        };

        if (!string.IsNullOrEmpty(correlationId))
        {
            details["correlationId"] = correlationId;
        }

        return await CreateAuditEntryAsync(
            EntityTypeIdentity,
            identityId,
            "IdentityVerified",
            SourceService,
            details,
            cancellationToken);
    }

    public async Task<AuditTrailResult> GetAuditTrailAsync(
        string entityType,
        string entityId,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "Getting audit trail: EntityType={EntityType}, EntityId={EntityId}",
            entityType, entityId);

        try
        {
            var response = await _complianceClient.GetAuditTrailAsync(
                entityType, entityId, limit, offset, cancellationToken);

            if (response.Success)
            {
                var entries = response.Entries.Select(e => new AuditEntryInfo(
                    AuditEntryId: e.AuditEntryId,
                    EntityType: e.EntityType,
                    EntityId: e.EntityId,
                    Action: e.Action,
                    Actor: e.Actor,
                    Timestamp: DateTimeOffset.FromUnixTimeSeconds(e.Timestamp).UtcDateTime,
                    Details: e.Details)).ToList();

                return new AuditTrailResult(
                    Entries: entries,
                    TotalCount: response.TotalCount,
                    Success: true,
                    ErrorMessage: null);
            }

            return new AuditTrailResult(
                Entries: Array.Empty<AuditEntryInfo>(),
                TotalCount: 0,
                Success: false,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting audit trail for {EntityType}:{EntityId}", entityType, entityId);
            return new AuditTrailResult(
                Entries: Array.Empty<AuditEntryInfo>(),
                TotalCount: 0,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    public async Task<KycStatusResult> UpdateKycStatusAsync(
        string identityId,
        string kycLevel,
        string status,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating KYC status: IdentityId={IdentityId}, Level={Level}, Status={Status}",
            identityId, kycLevel, status);

        try
        {
            var response = await _complianceClient.UpdateKycStatusAsync(
                identityId, kycLevel, status, cancellationToken);

            return new KycStatusResult(
                Success: response.Success,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating KYC status for {IdentityId}", identityId);
            return new KycStatusResult(false, ex.Message);
        }
    }

    public async Task<KycStatusInfo?> GetKycStatusAsync(
        string identityId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting KYC status for identity: {IdentityId}", identityId);

        try
        {
            var response = await _complianceClient.GetKycStatusAsync(identityId, cancellationToken);

            if (response == null || !response.Success)
            {
                return null;
            }

            return new KycStatusInfo(
                KycLevel: response.KycLevel,
                Status: response.Status,
                VerifiedAt: response.VerifiedAt.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(response.VerifiedAt.Value).UtcDateTime
                    : null,
                ExpiresAt: response.ExpiresAt.HasValue
                    ? DateTimeOffset.FromUnixTimeSeconds(response.ExpiresAt.Value).UtcDateTime
                    : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting KYC status for {IdentityId}", identityId);
            return null;
        }
    }

    public async Task<RedFlagResult> CreateRedFlagAsync(
        string identityId,
        string flagType,
        string severity,
        string description,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogWarning(
            "Creating red flag for identity: IdentityId={IdentityId}, Type={Type}, Severity={Severity}",
            identityId, flagType, severity);

        try
        {
            var response = await _complianceClient.CreateRedFlagAsync(
                identityId, flagType, severity, description, metadata, cancellationToken);

            return new RedFlagResult(
                Success: response.Success,
                RedFlagId: response.RedFlagId,
                ErrorMessage: response.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating red flag for {IdentityId}", identityId);
            return new RedFlagResult(false, null, ex.Message);
        }
    }
}
