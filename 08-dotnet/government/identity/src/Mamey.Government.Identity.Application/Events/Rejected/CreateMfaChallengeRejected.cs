using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.Government.Identity.Application.Events.Rejected;

[Contract]
internal record CreateMfaChallengeRejected(Guid ChallengeId, string Reason, string Code) : IRejectedEvent;
