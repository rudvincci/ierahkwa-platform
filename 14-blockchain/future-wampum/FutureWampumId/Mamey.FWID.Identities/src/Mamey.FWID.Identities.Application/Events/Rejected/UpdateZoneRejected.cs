using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Rejected;

[Contract]
internal record UpdateZoneRejected(Guid IdentityId, string Reason, string Code) : IRejectedEvent;

