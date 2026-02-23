using Mamey.CQRS;
using Pupitre.Accessibility.Domain.Entities;

namespace Pupitre.Accessibility.Domain.Events;

internal record AccessProfileRemoved(AccessProfile AccessProfile) : IDomainEvent;