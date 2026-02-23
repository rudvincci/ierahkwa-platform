using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIContent.Application.Events;

[Contract]
internal record ContentGenerationUpdated(Guid ContentGenerationId) : IEvent;


