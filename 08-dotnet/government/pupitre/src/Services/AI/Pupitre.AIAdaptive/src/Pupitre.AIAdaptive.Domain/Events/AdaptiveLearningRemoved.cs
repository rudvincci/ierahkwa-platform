using Mamey.CQRS;
using Pupitre.AIAdaptive.Domain.Entities;

namespace Pupitre.AIAdaptive.Domain.Events;

internal record AdaptiveLearningRemoved(AdaptiveLearning AdaptiveLearning) : IDomainEvent;