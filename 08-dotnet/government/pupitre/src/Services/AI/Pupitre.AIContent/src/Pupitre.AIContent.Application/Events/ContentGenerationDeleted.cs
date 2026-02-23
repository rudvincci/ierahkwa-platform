using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIContent.Application.Events;

[Contract]
internal record ContentGenerationDeleted(Guid ContentGenerationId) : IEvent;


