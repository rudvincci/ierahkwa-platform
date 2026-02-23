using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a DID resolution is requested.
/// </summary>
internal record DIDResolutionRequested(
    DIDResolutionId DIDResolutionId,
    string DID,
    string RequestedBy,
    ResolutionType ResolutionType,
    DateTime RequestedAt) : IDomainEvent;

/// <summary>
/// Event raised when DID resolution starts.
/// </summary>
internal record DIDResolutionStarted(
    DIDResolutionId DIDResolutionId,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when a resolution attempt is recorded.
/// </summary>
internal record ResolutionAttemptRecorded(
    DIDResolutionId DIDResolutionId,
    string Method,
    ResolutionAttemptResult Result,
    DateTime AttemptedAt) : IDomainEvent;

/// <summary>
/// Event raised when DID resolution completes successfully.
/// </summary>
internal record DIDResolutionCompleted(
    DIDResolutionId DIDResolutionId,
    string BlockchainTxHash,
    long BlockNumber,
    int ConfidenceScore,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when DID resolution fails.
/// </summary>
internal record DIDResolutionFailed(
    DIDResolutionId DIDResolutionId,
    string Reason,
    DateTime FailedAt) : IDomainEvent;

/// <summary>
/// Event raised when DID resolution uses cached data.
/// </summary>
internal record DIDResolutionFromCache(
    DIDResolutionId DIDResolutionId,
    DateTime CacheTimestamp,
    int HitCount) : IDomainEvent;
