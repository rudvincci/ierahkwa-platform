using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAdaptive.Application.Events;

[Contract]
internal record AdaptiveLearningDeleted(Guid AdaptiveLearningId) : IEvent;


