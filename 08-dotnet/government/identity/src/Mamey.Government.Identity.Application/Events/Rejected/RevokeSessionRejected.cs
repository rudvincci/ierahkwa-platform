using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record RevokeSessionRejected(Guid SessionId, string Reason, string Code) : IRejectedEvent;
