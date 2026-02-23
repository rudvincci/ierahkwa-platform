using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Mamey.FWID.Identities.Application.Events;

[Contract]
internal record IdentityAdded(Guid IdentityId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

