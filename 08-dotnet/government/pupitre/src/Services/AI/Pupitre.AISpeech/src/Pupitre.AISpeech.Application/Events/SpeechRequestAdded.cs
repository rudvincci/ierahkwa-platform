using System.Runtime.CompilerServices;
using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;
using Mamey.Types;


namespace Pupitre.AISpeech.Application.Events;

[Contract]
internal record SpeechRequestAdded(Guid SpeechRequestId, string Name, string Email, Owner owner, string? EncryptedPassword = null) : IEvent;

