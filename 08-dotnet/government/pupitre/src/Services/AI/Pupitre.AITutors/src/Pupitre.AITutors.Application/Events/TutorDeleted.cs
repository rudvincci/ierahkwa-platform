using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AITutors.Application.Events;

[Contract]
internal record TutorDeleted(Guid TutorId) : IEvent;


