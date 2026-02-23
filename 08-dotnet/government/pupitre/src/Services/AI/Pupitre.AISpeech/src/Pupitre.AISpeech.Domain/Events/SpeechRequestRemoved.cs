using Mamey.CQRS;
using Pupitre.AISpeech.Domain.Entities;

namespace Pupitre.AISpeech.Domain.Events;

internal record SpeechRequestRemoved(SpeechRequest SpeechRequest) : IDomainEvent;