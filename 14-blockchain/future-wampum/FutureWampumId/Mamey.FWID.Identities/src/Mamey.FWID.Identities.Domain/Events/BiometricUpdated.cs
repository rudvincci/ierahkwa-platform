using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when biometric data is updated.
/// </summary>
[Contract]
internal record BiometricUpdated(IdentityId IdentityId, DateTime UpdatedAt) : IDomainEvent;

