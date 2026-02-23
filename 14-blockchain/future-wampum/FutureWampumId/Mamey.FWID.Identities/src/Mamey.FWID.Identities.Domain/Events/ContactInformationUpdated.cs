using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when contact information is updated.
/// </summary>
[Contract]
internal record ContactInformationUpdated(IdentityId IdentityId, DateTime UpdatedAt) : IDomainEvent;

