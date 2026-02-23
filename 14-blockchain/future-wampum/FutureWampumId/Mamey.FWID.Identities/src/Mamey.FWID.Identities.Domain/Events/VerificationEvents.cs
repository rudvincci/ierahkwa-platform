using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a verification session is started.
/// </summary>
internal record VerificationSessionStarted(
    VerificationSessionId VerificationSessionId,
    IdentityId IdentityId,
    VerificationType VerificationType,
    string RequestedBy,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when a document is added to a verification session.
/// </summary>
internal record DocumentAddedToVerification(
    VerificationSessionId VerificationSessionId,
    string DocumentId,
    DocumentType DocumentType,
    string FileName) : IDomainEvent;

/// <summary>
/// Event raised when a biometric sample is added to a verification session.
/// </summary>
internal record BiometricSampleAdded(
    VerificationSessionId VerificationSessionId,
    string SampleId,
    BiometricType BiometricType) : IDomainEvent;

/// <summary>
/// Event raised when verification processing starts.
/// </summary>
internal record VerificationProcessingStarted(
    VerificationSessionId VerificationSessionId) : IDomainEvent;

/// <summary>
/// Event raised when a verification step is completed.
/// </summary>
internal record VerificationStepCompleted(
    VerificationSessionId VerificationSessionId,
    string StepName,
    VerificationStepResult Result,
    int ConfidenceScore) : IDomainEvent;

/// <summary>
/// Event raised when a verification session is completed.
/// </summary>
internal record VerificationSessionCompleted(
    VerificationSessionId VerificationSessionId,
    VerificationResult Result,
    int ConfidenceScore,
    string? Reason,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when a verification session fails.
/// </summary>
internal record VerificationSessionFailed(
    VerificationSessionId VerificationSessionId,
    string Reason,
    DateTime FailedAt) : IDomainEvent;
