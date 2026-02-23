using Mamey.CQRS;
using Pupitre.Educators.Domain.Entities;

namespace Pupitre.Educators.Domain.Events;

internal record EducatorRemoved(Educator Educator) : IDomainEvent;