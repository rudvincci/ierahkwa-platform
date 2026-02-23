using Mamey.CQRS;
using Pupitre.AITutors.Domain.Entities;

namespace Pupitre.AITutors.Domain.Events;

internal record TutorCreated(Tutor Tutor) : IDomainEvent;

