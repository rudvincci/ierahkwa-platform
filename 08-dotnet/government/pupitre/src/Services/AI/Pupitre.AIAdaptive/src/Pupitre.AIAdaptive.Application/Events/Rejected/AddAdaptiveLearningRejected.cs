using Mamey.CQRS.Events;
using Mamey.Microservice.Abstractions.Messaging;

namespace Pupitre.AIAdaptive.Application.Events.Rejected;

[Contract]
internal record AddAdaptiveLearningRejected(Guid AdaptiveLearningId, string Reason, string Code) : IRejectedEvent;
