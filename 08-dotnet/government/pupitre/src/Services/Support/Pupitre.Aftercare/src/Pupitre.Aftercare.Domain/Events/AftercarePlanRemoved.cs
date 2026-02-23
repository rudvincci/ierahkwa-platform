using Mamey.CQRS;
using Pupitre.Aftercare.Domain.Entities;

namespace Pupitre.Aftercare.Domain.Events;

internal record AftercarePlanRemoved(AftercarePlan AftercarePlan) : IDomainEvent;