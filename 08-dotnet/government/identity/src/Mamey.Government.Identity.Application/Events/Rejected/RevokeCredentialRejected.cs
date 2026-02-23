using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record RevokeCredentialRejected(Guid CredentialId, string Reason, string Code) : IRejectedEvent;
