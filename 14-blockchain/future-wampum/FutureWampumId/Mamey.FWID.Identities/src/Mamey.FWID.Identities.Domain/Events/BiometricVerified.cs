using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when biometric data is verified for an identity.
/// Compliant with Biometric Verification Microservice spec (§6).
/// </summary>
[Contract]
internal record BiometricVerified(
    IdentityId IdentityId,
    string? Did,
    double Similarity,
    double? LivenessScore,
    string Decision, // PASS | FAIL | INCONCLUSIVE
    string EvidenceJws,
    DateTime VerifiedAt) : IDomainEvent;

