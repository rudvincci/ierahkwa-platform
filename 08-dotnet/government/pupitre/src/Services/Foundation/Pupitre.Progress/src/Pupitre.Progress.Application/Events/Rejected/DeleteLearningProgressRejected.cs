using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.Progress.Application.Events.Rejected;

[Contract]
internal record DeleteLearningProgressRejected(Guid LearningProgressId, string Reason, string Code) : IRejectedEvent;