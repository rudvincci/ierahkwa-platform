using Mamey.CQRS;
using Pupitre.Progress.Domain.Entities;

namespace Pupitre.Progress.Domain.Events;

internal record LearningProgressRemoved(LearningProgress LearningProgress) : IDomainEvent;