namespace Mamey.FWID.Identities.Application.Clients;

/// <summary>
/// Client interface for MameyNode ComplianceService operations.
/// 
/// TDD Reference: Lines 1476-1498 (Compliance Requirements)
/// BDD Reference: Lines 645-692 (VII. Compliance and Regulatory Framework)
/// </summary>
internal interface IComplianceClient
{
    /// <summary>
    /// Creates an audit entry in the compliance system.
    /// </summary>
    Task<CreateAuditEntryResponse> CreateAuditEntryAsync(
        CreateAuditEntryRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the audit trail for an entity.
    /// </summary>
    Task<GetAuditTrailResponse> GetAuditTrailAsync(
        string entityType,
        string entityId,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies KYC for an account.
    /// </summary>
    Task<VerifyKycResponse> VerifyKycAsync(
        string accountId,
        string verificationType,
        Dictionary<string, string>? verificationData = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates KYC status for an account.
    /// </summary>
    Task<UpdateKycStatusResponse> UpdateKycStatusAsync(
        string accountId,
        string kycLevel,
        string status,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets KYC status for an account.
    /// </summary>
    Task<GetKycStatusResponse?> GetKycStatusAsync(
        string accountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks AML for an account/transaction.
    /// </summary>
    Task<CheckAmlResponse> CheckAmlAsync(
        string accountId,
        string? transactionId = null,
        string? amount = null,
        string? currency = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a red flag for suspicious activity.
    /// </summary>
    Task<CreateRedFlagResponse> CreateRedFlagAsync(
        string accountId,
        string flagType,
        string severity,
        string description,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Request to create an audit entry.
/// </summary>
internal record CreateAuditEntryRequest(
    string EntityType,
    string EntityId,
    string Action,
    string Actor,
    Dictionary<string, string>? Details = null);

/// <summary>
/// Response from creating an audit entry.
/// </summary>
internal record CreateAuditEntryResponse(
    bool Success,
    string? AuditEntryId,
    string? ErrorMessage);

/// <summary>
/// Response from getting the audit trail.
/// </summary>
internal record GetAuditTrailResponse(
    IReadOnlyList<AuditEntryDto> Entries,
    int TotalCount,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Audit entry data transfer object.
/// </summary>
internal record AuditEntryDto(
    string AuditEntryId,
    string EntityType,
    string EntityId,
    string Action,
    string Actor,
    long Timestamp,
    Dictionary<string, string> Details);

/// <summary>
/// Response from KYC verification.
/// </summary>
internal record VerifyKycResponse(
    bool Verified,
    string? KycLevel,
    IReadOnlyList<string> VerifiedAttributes,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response from updating KYC status.
/// </summary>
internal record UpdateKycStatusResponse(
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response from getting KYC status.
/// </summary>
internal record GetKycStatusResponse(
    string KycLevel,
    string Status,
    long? VerifiedAt,
    long? ExpiresAt,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response from AML check.
/// </summary>
internal record CheckAmlResponse(
    bool Flagged,
    string? RiskLevel,
    IReadOnlyList<string> RiskFactors,
    bool Success,
    string? ErrorMessage);

/// <summary>
/// Response from creating a red flag.
/// </summary>
internal record CreateRedFlagResponse(
    bool Success,
    string? RedFlagId,
    string? ErrorMessage);
