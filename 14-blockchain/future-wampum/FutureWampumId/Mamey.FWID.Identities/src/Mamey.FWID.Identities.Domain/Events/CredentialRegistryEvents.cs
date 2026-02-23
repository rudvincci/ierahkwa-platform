using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a credential is registered.
/// </summary>
internal record CredentialRegistered(
    CredentialRegistryId CredentialRegistryId,
    string CredentialId,
    string IssuerDid,
    string CredentialType,
    DateTime RegisteredAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential is anchored to the blockchain.
/// </summary>
internal record CredentialAnchored(
    CredentialRegistryId CredentialRegistryId,
    string TransactionHash,
    long BlockNumber,
    DateTime AnchoredAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential is verified.
/// </summary>
internal record CredentialVerified(
    CredentialRegistryId CredentialRegistryId,
    string VerifierId,
    VerificationResult Result,
    int ConfidenceScore,
    DateTime VerifiedAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential is revoked.
/// </summary>
internal record CredentialRevoked(
    CredentialRegistryId CredentialRegistryId,
    string Reason,
    string RevokedBy,
    DateTime RevokedAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential is suspended.
/// </summary>
internal record CredentialSuspended(
    CredentialRegistryId CredentialRegistryId,
    string Reason,
    string SuspendedBy,
    DateTime SuspendedAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential is reactivated.
/// </summary>
internal record CredentialReactivated(
    CredentialRegistryId CredentialRegistryId,
    string ReactivatedBy,
    DateTime ReactivatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential expiration is updated.
/// </summary>
internal record CredentialExpirationUpdated(
    CredentialRegistryId CredentialRegistryId,
    DateTime NewExpirationDate,
    DateTime UpdatedAt) : IDomainEvent;
