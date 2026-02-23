using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events;

[Contract]
internal record IdentityUpdated(Guid IdentityId) : IEvent;


