using Mamey.CQRS;
using Pupitre.Curricula.Domain.Entities;

namespace Pupitre.Curricula.Domain.Events;

internal record CurriculumModified(Curriculum Curriculum): IDomainEvent;

