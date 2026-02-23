using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITutors.Application.Events.Rejected;

[Contract]
internal record AddTutorRejected(Guid TutorId, string Reason, string Code) : IRejectedEvent;
