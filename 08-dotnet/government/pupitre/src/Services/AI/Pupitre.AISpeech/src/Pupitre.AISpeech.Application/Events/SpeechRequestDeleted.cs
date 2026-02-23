using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISpeech.Application.Events;

[Contract]
internal record SpeechRequestDeleted(Guid SpeechRequestId) : IEvent;


