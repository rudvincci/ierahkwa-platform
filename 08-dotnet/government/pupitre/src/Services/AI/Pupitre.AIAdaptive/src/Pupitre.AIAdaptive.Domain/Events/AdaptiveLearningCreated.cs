using Mamey.CQRS;
using Pupitre.AIAdaptive.Domain.Entities;

namespace Pupitre.AIAdaptive.Domain.Events;

internal record AdaptiveLearningCreated(AdaptiveLearning AdaptiveLearning) : IDomainEvent;

