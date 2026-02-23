using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITutors.Application.Events;

[Contract]
internal record TutorUpdated(Guid TutorId) : IEvent;


