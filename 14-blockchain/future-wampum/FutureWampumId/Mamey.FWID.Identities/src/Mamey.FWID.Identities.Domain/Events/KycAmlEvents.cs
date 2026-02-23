using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a KYC/AML profile is created.
/// </summary>
internal record KycAmlProfileCreated(
    KycAmlProfileId KycAmlProfileId,
    IdentityId IdentityId,
    string Jurisdiction,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when KYC/AML verification is started.
/// </summary>
internal record KycAmlVerificationStarted(
    KycAmlProfileId KycAmlProfileId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when a risk assessment is added.
/// </summary>
internal record RiskAssessmentAdded(
    KycAmlProfileId KycAmlProfileId,
    string AssessmentType,
    RiskLevel RiskLevel,
    int Score) : IDomainEvent;

/// <summary>
/// Event raised when sanctions screening is completed.
/// </summary>
internal record SanctionsScreeningCompleted(
    KycAmlProfileId KycAmlProfileId,
    string ScreeningProvider,
    int MatchCount,
    int ConfidenceScore) : IDomainEvent;

/// <summary>
/// Event raised when PEP screening is completed.
/// </summary>
internal record PepScreeningCompleted(
    KycAmlProfileId KycAmlProfileId,
    string ScreeningProvider,
    int MatchCount,
    int ConfidenceScore) : IDomainEvent;

/// <summary>
/// Event raised when an adverse media finding is added.
/// </summary>
internal record AdverseMediaFindingAdded(
    KycAmlProfileId KycAmlProfileId,
    string Source,
    AdverseMediaSeverity Severity) : IDomainEvent;

/// <summary>
/// Event raised when a compliance check is performed.
/// </summary>
internal record ComplianceCheckPerformed(
    KycAmlProfileId KycAmlProfileId,
    string CheckType,
    ComplianceResult Result) : IDomainEvent;

/// <summary>
/// Event raised when a KYC/AML profile is approved.
/// </summary>
internal record KycAmlProfileApproved(
    KycAmlProfileId KycAmlProfileId,
    string ApprovedBy,
    DateTime ExpiresAt) : IDomainEvent;

/// <summary>
/// Event raised when a KYC/AML profile is rejected.
/// </summary>
internal record KycAmlProfileRejected(
    KycAmlProfileId KycAmlProfileId,
    string Reason,
    string RejectedBy,
    DateTime RejectedAt) : IDomainEvent;
