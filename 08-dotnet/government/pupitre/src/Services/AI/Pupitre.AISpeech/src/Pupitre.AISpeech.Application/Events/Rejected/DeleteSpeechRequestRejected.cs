using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISpeech.Application.Events.Rejected;

[Contract]
internal record DeleteSpeechRequestRejected(Guid SpeechRequestId, string Reason, string Code) : IRejectedEvent;