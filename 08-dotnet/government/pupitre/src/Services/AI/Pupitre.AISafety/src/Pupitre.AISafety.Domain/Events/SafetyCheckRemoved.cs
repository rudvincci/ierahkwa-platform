using Mamey.CQRS;
using Pupitre.AISafety.Domain.Entities;

namespace Pupitre.AISafety.Domain.Events;

internal record SafetyCheckRemoved(SafetyCheck SafetyCheck) : IDomainEvent;