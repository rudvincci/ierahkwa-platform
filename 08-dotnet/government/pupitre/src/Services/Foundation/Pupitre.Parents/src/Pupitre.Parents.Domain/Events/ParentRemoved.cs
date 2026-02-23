using Mamey.CQRS;
using Pupitre.Parents.Domain.Entities;

namespace Pupitre.Parents.Domain.Events;

internal record ParentRemoved(Parent Parent) : IDomainEvent;