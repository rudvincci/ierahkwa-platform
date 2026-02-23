using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITranslation.Application.Events;

[Contract]
internal record TranslationRequestUpdated(Guid TranslationRequestId) : IEvent;


