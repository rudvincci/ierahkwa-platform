using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when biometric data is enrolled for an identity.
/// Compliant with Biometric Verification Microservice spec (§6).
/// </summary>
[Contract]
internal record BiometricEnrolled(
    IdentityId IdentityId,
    string? Did,
    string TemplateId,
    string EvidenceJws,
    string AlgoVersion,
    BiometricQuality Quality,
    double? LivenessScore,
    string? LivenessDecision,
    DateTime EnrolledAt) : IDomainEvent;

