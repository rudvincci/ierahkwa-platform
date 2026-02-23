using Mamey.CQRS;
using Pupitre.GLEs.Domain.Entities;

namespace Pupitre.GLEs.Domain.Events;

internal record GLEModified(GLE GLE): IDomainEvent;

