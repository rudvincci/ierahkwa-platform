using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISpeech.Application.Events;

[Contract]
internal record SpeechRequestUpdated(Guid SpeechRequestId) : IEvent;


