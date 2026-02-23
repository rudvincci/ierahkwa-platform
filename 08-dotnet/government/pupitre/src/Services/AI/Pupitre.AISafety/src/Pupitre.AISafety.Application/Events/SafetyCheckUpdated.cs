using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISafety.Application.Events;

[Contract]
internal record SafetyCheckUpdated(Guid SafetyCheckId) : IEvent;


