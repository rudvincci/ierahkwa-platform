using Mamey.CQRS;
using Pupitre.Progress.Domain.Entities;

namespace Pupitre.Progress.Domain.Events;

internal record LearningProgressCreated(LearningProgress LearningProgress) : IDomainEvent;

