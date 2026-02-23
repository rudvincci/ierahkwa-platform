using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITranslation.Application.Events;

[Contract]
internal record TranslationRequestDeleted(Guid TranslationRequestId) : IEvent;


