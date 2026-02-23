using Mamey.CQRS;
using Pupitre.Ministries.Domain.Entities;

namespace Pupitre.Ministries.Domain.Events;

internal record MinistryDataRemoved(MinistryData MinistryData) : IDomainEvent;