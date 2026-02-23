using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when an encryption policy is created.
/// </summary>
internal record EncryptionPolicyCreated(
    EncryptionId EncryptionId,
    string EntityType,
    string EntityId,
    EncryptionLevel EncryptionLevel,
    DateTime CreatedAt) : IDomainEvent;

/// <summary>
/// Event raised when encryption starts.
/// </summary>
internal record EncryptionStarted(
    EncryptionId EncryptionId,
    string Algorithm,
    int KeySize,
    DateTime StartedAt) : IDomainEvent;

/// <summary>
/// Event raised when encryption completes.
/// </summary>
internal record EncryptionCompleted(
    EncryptionId EncryptionId,
    int EncryptedFieldCount,
    DateTime CompletedAt) : IDomainEvent;

/// <summary>
/// Event raised when an access policy is granted.
/// </summary>
internal record AccessPolicyGranted(
    EncryptionId EncryptionId,
    string PolicyId,
    string UserId,
    List<string> Permissions,
    DateTime? ExpiresAt) : IDomainEvent;

/// <summary>
/// Event raised when data is accessed (decrypted).
/// </summary>
internal record DataAccessed(
    EncryptionId EncryptionId,
    string UserId,
    string FieldName,
    string Purpose,
    DateTime AccessedAt) : IDomainEvent;

/// <summary>
/// Event raised when encryption keys are rotated.
/// </summary>
internal record KeyRotated(
    EncryptionId EncryptionId,
    string OldKeyId,
    string NewKeyId,
    string RotatedBy,
    DateTime RotatedAt) : IDomainEvent;
