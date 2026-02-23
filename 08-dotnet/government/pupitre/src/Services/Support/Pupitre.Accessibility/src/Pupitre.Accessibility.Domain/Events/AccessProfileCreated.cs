using Mamey.CQRS;
using Pupitre.Accessibility.Domain.Entities;

namespace Pupitre.Accessibility.Domain.Events;

internal record AccessProfileCreated(AccessProfile AccessProfile) : IDomainEvent;

