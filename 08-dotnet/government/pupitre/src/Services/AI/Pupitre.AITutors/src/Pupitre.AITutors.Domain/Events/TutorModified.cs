using Mamey.CQRS;
using Pupitre.AITutors.Domain.Entities;

namespace Pupitre.AITutors.Domain.Events;

internal record TutorModified(Tutor Tutor): IDomainEvent;

