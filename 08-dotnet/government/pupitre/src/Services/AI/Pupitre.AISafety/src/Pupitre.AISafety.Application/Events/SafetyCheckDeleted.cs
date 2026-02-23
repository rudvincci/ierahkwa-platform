using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISafety.Application.Events;

[Contract]
internal record SafetyCheckDeleted(Guid SafetyCheckId) : IEvent;


