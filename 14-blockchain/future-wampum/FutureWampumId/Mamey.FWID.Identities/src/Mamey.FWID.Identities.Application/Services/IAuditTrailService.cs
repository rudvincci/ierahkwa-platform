namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service interface for compliance audit trail operations on the blockchain.
/// Provides AML/KYC audit logging for regulatory compliance (2025-AM01, 2025-ID01).
/// 
/// TDD Reference: Lines 1476-1498 (Compliance Requirements)
/// BDD Reference: Lines 645-692 (VII. Compliance and Regulatory Framework)
/// </summary>
internal interface IAuditTrailService
{
    /// <summary>
    /// Creates an audit entry for identity-related actions.
    /// </summary>
    Task<AuditEntryResult> CreateAuditEntryAsync(
        string entityType,
        string entityId,
        string action,
        string actor,
        Dictionary<string, string>? details = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs a KYC verification event to the compliance audit trail.
    /// </summary>
    Task<AuditEntryResult> LogKycVerificationAsync(
        string identityId,
        string verificationType,
        bool verified,
        string kycLevel,
        IReadOnlyList<string> verifiedAttributes,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an AML check event to the compliance audit trail.
    /// </summary>
    Task<AuditEntryResult> LogAmlCheckAsync(
        string identityId,
        string? transactionId,
        bool flagged,
        string riskLevel,
        IReadOnlyList<string>? riskFactors = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an identity creation event for regulatory compliance.
    /// </summary>
    Task<AuditEntryResult> LogIdentityCreatedAsync(
        string identityId,
        string? zone,
        string? clanRegistrarId,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Logs an identity verification event for regulatory compliance.
    /// </summary>
    Task<AuditEntryResult> LogIdentityVerifiedAsync(
        string identityId,
        string verificationType,
        bool verified,
        string? correlationId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the audit trail for an identity.
    /// </summary>
    Task<AuditTrailResult> GetAuditTrailAsync(
        string entityType,
        string entityId,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates KYC status on the blockchain compliance system.
    /// </summary>
    Task<KycStatusResult> UpdateKycStatusAsync(
        string identityId,
        string kycLevel,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current KYC status for an identity.
    /// </summary>
    Task<KycStatusInfo?> GetKycStatusAsync(
        string identityId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a red flag for suspicious activity.
    /// </summary>
    Task<RedFlagResult> CreateRedFlagAsync(
        string identityId,
        string flagType,
        string severity,
        string description,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of creating an audit entry.
/// </summary>
internal record AuditEntryResult(
    bool Success,
    string? AuditEntryId,
    string? ErrorMessage);

/// <summary>
/// Result of retrieving the audit trail.
/// </summary>
internal record AuditTrailResult(
    IReadOnlyList<AuditEntryInfo> Entries,
    int TotalCount,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Information about a single audit entry.
/// </summary>
internal record AuditEntryInfo(
    string AuditEntryId,
    string EntityType,
    string EntityId,
    string Action,
    string Actor,
    DateTime Timestamp,
    Dictionary<string, string> Details);

/// <summary>
/// Result of updating KYC status.
/// </summary>
internal record KycStatusResult(
    bool Success,
    string? ErrorMessage);

/// <summary>
/// KYC status information.
/// </summary>
internal record KycStatusInfo(
    string KycLevel,
    string Status,
    DateTime? VerifiedAt,
    DateTime? ExpiresAt);

/// <summary>
/// Result of creating a red flag.
/// </summary>
internal record RedFlagResult(
    bool Success,
    string? RedFlagId,
    string? ErrorMessage);
