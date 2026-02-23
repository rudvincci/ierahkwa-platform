using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AISafety.Application.Events.Rejected;

[Contract]
internal record AddSafetyCheckRejected(Guid SafetyCheckId, string Reason, string Code) : IRejectedEvent;
