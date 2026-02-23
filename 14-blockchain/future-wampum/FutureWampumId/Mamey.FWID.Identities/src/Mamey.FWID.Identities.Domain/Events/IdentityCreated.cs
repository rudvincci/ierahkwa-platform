using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Domain event raised when an identity is created.
/// Per TDD: This event triggers DID creation and credential issuance.
/// </summary>
[Contract]
internal record IdentityCreated(
    IdentityId IdentityId, 
    Name Name, 
    DateTime CreatedAt, 
    string? Zone,
    string? BlockchainAccount = null) : IDomainEvent;

