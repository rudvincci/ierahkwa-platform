using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record EnableMfaMethodRejected(Guid UserId, int Method, string Reason, string Code) : IRejectedEvent;
