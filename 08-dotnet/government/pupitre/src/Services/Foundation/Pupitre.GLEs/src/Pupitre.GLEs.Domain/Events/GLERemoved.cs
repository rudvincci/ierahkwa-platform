using Mamey.CQRS;
using Pupitre.GLEs.Domain.Entities;

namespace Pupitre.GLEs.Domain.Events;

internal record GLERemoved(GLE GLE) : IDomainEvent;