using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Progress.Application.Events;

[Contract]
internal record LearningProgressDeleted(Guid LearningProgressId) : IEvent;


