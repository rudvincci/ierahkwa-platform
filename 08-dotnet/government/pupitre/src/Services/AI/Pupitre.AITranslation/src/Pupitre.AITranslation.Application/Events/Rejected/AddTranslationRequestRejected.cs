using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITranslation.Application.Events.Rejected;

[Contract]
internal record AddTranslationRequestRejected(Guid TranslationRequestId, string Reason, string Code) : IRejectedEvent;
