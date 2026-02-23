using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIContent.Application.Events.Rejected;

[Contract]
internal record DeleteContentGenerationRejected(Guid ContentGenerationId, string Reason, string Code) : IRejectedEvent;