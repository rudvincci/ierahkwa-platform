using Mamey.CQRS;
using Pupitre.AISpeech.Domain.Entities;

namespace Pupitre.AISpeech.Domain.Events;

internal record SpeechRequestModified(SpeechRequest SpeechRequest): IDomainEvent;

